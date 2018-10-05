using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JinZhou.Models;

namespace JinZhou.Services
{
    public class LogService
    {
        private static LogService logger = null;
        private static List<Log> logs = new List<Log>();
        private static object locker = new object();
        private LogService()
        {

        }

        public static LogService GetInstance()
        {
            if (logger == null)
            {
                lock (locker)
                {
                    if (logger == null)
                    {
                        logger = new LogService();
                    }
                }
            }

            return logger;
        }

        public void AddLog(Log log)
        {
            lock (locker)
            {
                logs.Add(log);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="where">所在函数名</param>
        /// <param name="who">记录IP or ThreadID</param>
        /// <param name="what"></param>
        /// <param name="why">记录exception等</param>
        /// <param name="how">记录日志等级</param>
        public void AddLog(string where, string who, string what, string why, string how)
        {
            if (string.IsNullOrEmpty(who))
            {
                who = "Thread" + Thread.CurrentThread.ManagedThreadId;
            }
            Log log = new Log()
            {
                When = DateTime.Now,
                Where = where,
                Who = who,
                What = what,
                Why = why,
                How = how
            };
            lock (locker)
            {
                logs.Add(log);
            }
        }

        public List<Log> GetLogs()
        {
            return logs;
        }
    }
}
