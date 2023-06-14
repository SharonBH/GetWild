using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Util;

namespace Utilities
{
    public static class Logger
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Logger()
        {
            XmlConfigurator.Configure();
        }

        //public static void logError(object msg, Exception ex)
        //{
        //    log.Error(msg, ex);
        //}

        #region Public Methods
        /// <summary>
        /// Write row to log with level of Debug
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteDebug(string text)
        {
            return WriteDebug(text, null);
        }

        /// <summary>
        /// Write row to log with level of Debug
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <param name="ex">Exception to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteDebug(string text, Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    log.Logger.Log(CreateLoggingEvent(text, null, Level.Debug));
                }
                else
                {
                    log.Logger.Log(CreateLoggingEvent(text, ex, Level.Debug));
                }
                return true;
            }
            catch
            {
            }

            return false;
        }

        public static void newInfo(string text)
        {
            log.Info(text);
        }

        /// <summary>
        /// Write row to log with level of Info
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteInfo(string text)
        {
            return WriteInfo(text, null);
        }

        /// <summary>
        /// Write row to log with level of Info
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <param name="ex">Exception to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteInfo(string text, Exception ex)
        {
            try
            {

                if (ex == null)
                {
                    log.Logger.Log(CreateLoggingEvent(text, null, Level.Info));

                }
                else
                {
                    log.Logger.Log(CreateLoggingEvent(text, ex, Level.Info));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Write row to log with level of Warn
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteWarn(string text)
        {
            return WriteWarn(text, null);
        }

        /// <summary>
        /// Write row to log with level of Warn
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <param name="ex">Exception to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteWarn(string text, Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    log.Logger.Log(CreateLoggingEvent(text, null, Level.Warn));
                }
                else
                {
                    log.Logger.Log(CreateLoggingEvent(text, ex, Level.Warn));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Write row to log with level of Error
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteError(string text)
        {
            return WriteError(text, null);
        }

        /// <summary>
        /// Write row to log with level of Error
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <param name="ex">Exception to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteError(string text, Exception ex)
        {
            try
            {
                log.Logger.Log(ex == null
                    ? CreateLoggingEvent(text, null, Level.Error)
                    : CreateLoggingEvent(text, ex, Level.Error));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Write row to log with level of Fatal
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <returns>true if successful</returns>        
        public static bool WriteFatal(string text)
        {
            return WriteFatal(text, null);
        }

        /// <summary>
        /// Write row to log with level of Fatal
        /// </summary>
        /// <param name="text">Message to log</param>
        /// <param name="ex">Exception to log</param>
        /// <returns>true if successful</returns>
        public static bool WriteFatal(string text, Exception ex)
        {
            try
            {
                if (ex == null)
                {
                    log.Logger.Log(CreateLoggingEvent(text, null, Level.Fatal));
                }
                else
                {
                    log.Logger.Log(CreateLoggingEvent(text, ex, Level.Fatal));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        private static LoggingEvent CreateLoggingEvent(string text, Exception ex, Level level)
        {
            try
            {

                LoggingEventData led = new LoggingEventData();

                #region Get LocationInfo and Domain
                //Default LocationInfo and Domain
                led.LocationInfo = new LocationInfo(Assembly.GetCallingAssembly().GetName().Name, string.Empty, string.Empty, string.Empty);
                led.Domain = Assembly.GetCallingAssembly().GetName().Name;

                //Try to get appropriate LocationInfo & Domain
                for (int i = 3; i >= 0; i--)
                {
                    try
                    {
                        StackFrame stf = new StackTrace(true).GetFrame(i);
                        MethodBase callingMethod = stf.GetMethod();
                        led.LocationInfo = new LocationInfo(callingMethod.ReflectedType.ToString(), callingMethod.Name, stf.GetFileName(), stf.GetFileLineNumber().ToString());
                        led.Domain = callingMethod.DeclaringType.Assembly.GetName().Name;
                        break;
                    }
                    catch
                    {
                    }
                }
                #endregion

                led.Properties = new PropertiesDictionary();
                led.Properties["execution_time"] = 0;
                led.ExceptionString = (ex == null ? string.Empty : ex.ToString());
                led.Level = level;
                led.LoggerName = log.Logger.Name;
                led.Message = text;
                led.ThreadName = Thread.CurrentThread.ManagedThreadId.ToString();
                led.TimeStampUtc = DateTime.Now;
                led.UserName = WindowsIdentity.GetCurrent().Name;
                return new LoggingEvent(led);
            }
            catch
            {
                return new LoggingEvent(new LoggingEventData());
            }
        }
    }
}
