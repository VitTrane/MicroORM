using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Logger
    {
        private ConsoleTraceListener _consoleTraceListener;
        private static Logger _instance=null;

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }

        private Logger()
        {
            _consoleTraceListener = new ConsoleTraceListener();
            Trace.Listeners.Add(_consoleTraceListener);
        }

        public void WriteLine(string message) 
        {
            Trace.WriteLine(message);
        }
    }
}
