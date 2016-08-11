using Configuration;
using ORMMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;
using Infrastructure;

namespace DataAccess
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, new()
    {
        private string _connectionString;
        private DbProviderFactory _dbFactory;
        private Mapper<TEntity> _mapper;

        public BaseRepository(string providerName, string connectionString)
        {
            _connectionString = connectionString;
            _dbFactory = DbProviderFactories.GetFactory(providerName);
            _mapper = new Mapper<TEntity>();
        }

        /// <summary>
        /// Возвращает объект из быза дынных по ключу
        /// </summary>
        /// <param name="key">Ключ по которому нужно найти объект</param>
        public TEntity GetById(TKey key)
        {
            TEntity entity = null;
            Dictionary<string, object> values = new Dictionary<string, object>();
            string tableName = _mapper.GetTableName();
            string columnPrimaryKeyName =_mapper.GetColumnPrimaryKeyName();
            string commandText = string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", tableName, columnPrimaryKeyName, key);
            entity = ExecuteReader(commandText).First();
            return entity;
        }        

        /// <summary>
        /// Обновляет объект в базе данных
        /// </summary>
        /// <param name="entity">Обновленный объект</param>
        public void Save(TEntity entity)
        {
            string tableName = _mapper.GetTableName();
            string columnPrimaryKeyName = _mapper.GetColumnPrimaryKeyName();
            var primaryKey = _mapper.GetPrimaryKey(entity);

            string commandText = string.Format("UPDATE {0} SET ", tableName);

            Dictionary<string, object> values = _mapper.GetPropertyValues(entity);
            foreach (var item in values)
            {
                commandText += item.Key + " = '" + item.Value + "' ,";
            }

            commandText = commandText.Remove(commandText.Length - 1);
            commandText += " WHERE " + columnPrimaryKeyName + " = " + primaryKey;

            ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// Удаляет объект из базы данных по ключу
        /// </summary>
        /// <param name="key">Ключ по которому нужно удалить объект</param>
        public void Delete(TKey key)
        {
            string tableName = _mapper.GetTableName();
            string columnPrimaryKeyName = _mapper.GetColumnPrimaryKeyName();
            string commandText = string.Format("DELETE FROM {0} WHERE {1} = {2}", tableName, columnPrimaryKeyName, key);
            ExecuteNonQuery(commandText);
        }

        private string GetCommandText(MemberExpression memberExpression, ConstantExpression constantExpression)
        {
            string tableName = _mapper.GetTableName();
            string propertyName = memberExpression.Member.Name;
            var columnName = _mapper.GetColumnName(propertyName);
            var value = Expression.Lambda(constantExpression).Compile().DynamicInvoke();
            return string.Format("SELECT * FROM {0} WHERE {1} = '{2}'", tableName, columnName, value);
        }

        public IEnumerable<TEntity> SelectWhere(Expression<Func<TEntity, bool>> lamda)
        {
            List<TEntity> entities = new List<TEntity>();

            string tableName = _mapper.GetTableName();
            string commandText = "";
            if (lamda.Body.NodeType == ExpressionType.Equal) 
            {
                BinaryExpression op = (BinaryExpression)lamda.Body;
                Expression left = op.Left;
                Expression right = op.Right;

                if ((left.NodeType == ExpressionType.MemberAccess) && (right.NodeType == ExpressionType.Constant))
                {
                    commandText = GetCommandText((MemberExpression)left, (ConstantExpression)right);
                }

                if ((right.NodeType == ExpressionType.MemberAccess) && (left.NodeType == ExpressionType.Constant))
                {
                    commandText = GetCommandText((MemberExpression)right, (ConstantExpression)left);
                }
            }

           return ExecuteReader(commandText);
        }        

        public void ExecuteNonQuery(string commandText) 
        {            
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                IDbCommand command = connection.CreateCommand();
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
            //Trace.WriteLine("Выполнился запрос: " + commandText);
            Logger.Instance.WriteLine("Выполнился запрос: " + commandText);
        }

        public IEnumerable<TEntity> ExecuteReader(string commandText) 
        {
            List<TEntity> entities = new List<TEntity>();
            
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                IDbCommand command = connection.CreateCommand();
                command.CommandText = commandText;

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            values.Add(reader.GetName(i), reader.GetValue(i));
                        }
                       TEntity entity = _mapper.Mapping(values);
                       entities.Add(entity);
                    }
                }
            }
            Logger.Instance.WriteLine("Выполнился запрос: " + commandText);
            return entities;
        }

        private DbConnection GetConnection()
        {
            DbConnection connection = _dbFactory.CreateConnection();
            connection.ConnectionString = _connectionString;
            return connection;
        }
    }
}
