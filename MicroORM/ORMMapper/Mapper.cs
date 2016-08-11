using Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ORMMapper
{
    public class Mapper<TEntity> where TEntity : class, new()
    {
        public Mapper()
        {
        }

        /// <summary>
        /// Возвращает название таблицы
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            Type typeEntity = typeof(TEntity);
            foreach (var attribute in typeEntity.CustomAttributes)
            {
                if (attribute.AttributeType == typeof(TableAttribute))
                {
                    TableAttribute tableAttribute = typeEntity.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
                    if (tableAttribute != null)
                        return tableAttribute.TableName;
                }
            }

            return "";
        }

        /// <summary>
        /// Возвращает название первичного ключа
        /// </summary>
        /// <returns></returns>
        public string GetColumnPrimaryKeyName()
        {
            Type typeEntity = typeof(TEntity);
            PropertyInfo[] props = typeEntity.GetProperties();
            foreach (var property in props)
            {
                foreach (var attribute in property.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(PrimaryKeyAttribute))
                    {
                        ColumnAttribute columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                        if (columnAttribute != null)
                            return columnAttribute.ColumnName;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Возвращает значение первичного ключа
        /// </summary>
        /// <param name="entity">Объект у которого нужно получить ключ</param>
        /// <returns></returns>
        public object GetPrimaryKey(TEntity entity)
        {
            Type typeEntity = typeof(TEntity);
            PropertyInfo[] props = typeEntity.GetProperties();
            foreach (var property in props)
            {
                foreach (var attribute in property.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(PrimaryKeyAttribute))
                    {
                        return property.GetValue(entity);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает название стобца
        /// </summary>
        /// <param name="propertyName">Название свойства, которое относится к нужному столбцу</param>
        /// <returns></returns>
        public string GetColumnName(string propertyName) 
        {
            Type typeEntity = typeof(TEntity);
            PropertyInfo property = typeEntity.GetProperty(propertyName);
            ColumnAttribute columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            return columnAttribute.ColumnName;
        }

        /// <summary>
        /// Соотносит данные полученные из бд с объектом
        /// </summary>
        /// <param name="values">Значение, которые нужно соотнести</param>
        /// <returns></returns>
        public TEntity Mapping(Dictionary<string, object> values)
        {            
            TEntity entity = new TEntity();
            Type typeEntity = typeof(TEntity);
            PropertyInfo[] props = typeEntity.GetProperties();
            foreach (var property in props)
            {
                foreach (var attribute in property.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(ColumnAttribute))
                    {
                        ColumnAttribute columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                        var result = Convert.ChangeType(values[columnAttribute.ColumnName], property.PropertyType);
                        property.SetValue(entity, result);
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Возвращает данные полученные из объекта
        /// </summary>
        /// <param name="entity">Объект, из которого нужно получить данные</param>
        /// <returns></returns>
        public Dictionary<string, object> GetPropertyValues(TEntity entity) 
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            Type typeEntity = typeof(TEntity);
            PropertyInfo[] props = typeEntity.GetProperties();
            foreach (var property in props)
            {
                foreach (var attribute in property.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(ColumnAttribute))
                    {
                        ColumnAttribute columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                        values.Add(columnAttribute.ColumnName, property.GetValue(entity));
                    }
                }
            }
            return values;
        }        
    }
}
