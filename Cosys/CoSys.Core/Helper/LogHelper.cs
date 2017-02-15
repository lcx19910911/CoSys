using System;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using NLog;
using System.Text;
using System.Web;

namespace CoSys.Core
{
    /// <summary>
    /// ��־��¼��
    /// </summary>
    public class LogHelper
    {
        private static bool isinit = false;

        static LogHelper()
        {
            if (isinit == false)
            {
                isinit = true;
                SetConfig();
            }
        }

        private static bool LogInfoEnable = false;
        private static bool LogErrorEnable = false;
        private static bool LogExceptionEnable = false;
        private static bool LogComplementEnable = false;
        private static bool LogDubugEnable = false;
        //private static bool LogFatalEnabled = false;

        private static Logger logger = LogManager.GetCurrentClassLogger();



        /// <summary>
        /// ���ó�ʼֵ��
        /// </summary>
        public static void SetConfig()
        {
            //log4net.Config.DOMConfigurator.Configure();
            //LogInfoEnable=LogInfo.IsInfoEnabled;
            //LogErrorEnable=LogError.IsErrorEnabled;
            //LogExceptionEnable=LogException.IsErrorEnabled;
            //LogComplementEnable=LogComplement.IsErrorEnabled;
            //LogDubugEnable = LogDubug.IsDebugEnabled;

            LogInfoEnable = logger.IsInfoEnabled;
            LogErrorEnable = logger.IsErrorEnabled;
            LogExceptionEnable = logger.IsErrorEnabled;
            LogComplementEnable = logger.IsTraceEnabled;
            //LogFatalEnabled = logger.IsFatalEnabled;
            LogDubugEnable = logger.IsDebugEnabled;

        }
        /// <summary>
        /// д����ͨ��־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfo(string info)
        {
            if (LogInfoEnable)
            {
                logger.Info(BuildMessage(info));
                //LogInfo.Info(info);
            }
        }
        /// <summary>
        /// д��Debug��־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public static void WriteDebug(string info)
        {
            if (LogDubugEnable)
            {
                logger.Debug(BuildMessage(info));
                //LogDubug.Debug(info);
            }
        }
        /// <summary>
        /// д�������־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public static void WriteError(string info)
        {
            if (LogErrorEnable)
            {
                logger.Error(BuildMessage(info));
                //LogError.Error(info);
            }
        }

        /// <summary>
        /// д���쳣��־��Ϣ
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="frameIndex"></param>
        public static void WriteException(Exception ex, int frameIndex = 0)
        {
            string info = string.Empty;
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            if (st.FrameCount > frameIndex)
            {
                info = st.GetFrame(frameIndex + 1).GetMethod().Name;
            }
            WriteException(info, ex);

        }

        /// <summary>
        /// д���쳣��־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        public static void WriteException(string info, Exception ex)
        {
            if (LogExceptionEnable)
            {
                logger.Error(BuildMessage(info, ex));
                //LogException.Error(info,ex);
            }
        }
        /// <summary>
        /// д�����ش�����־��Ϣ
        /// </summary>
        /// <param name="info"></param>
        public static void WriteFatal(string info)
        {
            if (LogErrorEnable)
            {
                logger.Fatal(BuildMessage(info));
            }
        }
        /// <summary>
        /// д�벹����־
        /// </summary>
        /// <param name="info"></param>
        public static void WriteComplement(string info)
        {
            if (LogComplementEnable)
            {
                logger.Trace(BuildMessage(info));
                //LogComplement.Error(info);
            }
        }
        /// <summary>
        /// д�벹����־
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        public static void WriteComplement(string info, Exception ex)
        {
            if (LogComplementEnable)
            {
                logger.Trace(BuildMessage(info, ex));
            }
        }


        static string BuildMessage(string info)
        {
            return BuildMessage(info, null);
        }

        static string BuildMessage(string info, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            HttpRequest request = null;
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                    request = HttpContext.Current.Request;
            }
            catch { }

            sb.AppendFormat("Time:{0}-{1}\r\n", DateTime.Now, info);

            if (request != null)
            {
                sb.AppendFormat("Url:{0}\r\n", request.Url);
                if (null != request.UrlReferrer)
                {
                    sb.AppendFormat("UrlReferrer:{0}\r\n", request.UrlReferrer);
                }
                string realip = request.ServerVariables == null
                                    ? string.Empty
                                    : request.ServerVariables["HTTP_X_REAL_IP"];
                string proxy = request.Headers == null
                                    ? string.Empty
                                    : request.Headers.Get("HTTP_NDUSER_FORWARDED_FOR_HAPROXY");
                sb.AppendFormat("UserHostAddress:{0};{1};{2}\r\n", request.UserHostAddress, realip, proxy);
                sb.AppendFormat("WebServer:{0}\r\n", request.ServerVariables["LOCAL_ADDR"]);
            }

            if (ex != null)
            {
                if (ex is SqlException)
                    sb.AppendFormat("Database:{0}\r\n", ((SqlException)ex).Server);
                sb.AppendFormat("Exception:{0}\r\n", ex);
            }
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// д���Զ�����־���Զ���Ŀ¼,��������Ӧ��Nlog.config����ʾ����
        ///  &lt;targets>
        ///    &lt;target name="LogCustom" xsi:type="File" layout="${message}"
        ///          fileName="${logDirectory}\${event-context:DirOrPrefix}${date:format=yyyyMMddHH}.txt">&lt;/target>
        ///  &lt;/targets>
        ///  &lt;rules>
        ///    &lt;logger name="LogCustom" level="Warn" writeTo="LogCustom" />
        /// </summary>
        /// <param name="message">Ҫд�����Ϣ</param>
        /// <param name="dirOrPrefix">
        /// д�뵽����Ŀ¼���ļ�ǰ׺������ַ�������\��������Ŀ¼
        /// ���� aa\bb ��д����ļ���ΪaaĿ¼�µ�bb��ͷ������
        /// </param>
        public static void WriteCustom(string message, string dirOrPrefix)
        {
            WriteCustom(message, dirOrPrefix, null, true);
        }

        /// <summary>
        /// д���Զ�����־���Զ���Ŀ¼,��������Ӧ��Nlog.config����ʾ����
        ///  &lt;targets>
        ///    &lt;target name="LogCustom" xsi:type="File" layout="${message}"
        ///          fileName="${logDirectory}\${event-context:DirOrPrefix}${date:format=yyyyMMddHH}.txt">&lt;/target>
        ///  &lt;/targets>
        ///  &lt;rules>
        ///    &lt;logger name="LogCustom" level="Warn" writeTo="LogCustom" />
        /// </summary>
        /// <param name="message">Ҫд�����Ϣ</param>
        /// <param name="dirOrPrefix">
        /// д�뵽����Ŀ¼���ļ�ǰ׺������ַ�������\��������Ŀ¼
        /// ���� aa\bb ��д����ļ���ΪaaĿ¼�µ�bb��ͷ������
        /// </param>
        /// <param name="addIpUrl">�Ƿ�Ҫ����ip��url����Ϣ</param>
        public static void WriteCustom(string message, string dirOrPrefix, bool addIpUrl)
        {
            WriteCustom(message, dirOrPrefix, null, addIpUrl);
        }


        /// <summary>
        /// д���Զ�����־���Զ���Ŀ¼,��������Ӧ��Nlog.config����ʾ����
        ///  &lt;targets>
        ///    &lt;target name="LogCustom" xsi:type="File" layout="${message}"
        ///          fileName="${logDirectory}\${event-context:DirOrPrefix}${date:format=yyyyMMddHH}${event-context:Suffix}.txt">&lt;/target>
        ///  &lt;/targets>
        ///  &lt;rules>
        ///    &lt;logger name="LogCustom" level="Warn" writeTo="LogCustom" />
        /// </summary>
        /// <param name="message">Ҫд�����Ϣ</param>
        /// <param name="dirOrPrefix">
        /// д�뵽����Ŀ¼���ļ�ǰ׺������ַ�������\��������Ŀ¼
        /// ���� aa\bb ��д����ļ���ΪaaĿ¼�µ�bb��ͷ������
        /// </param>
        /// <param name="suffix">д�뵽���ļ���׺</param>
        public static void WriteCustom(string message, string dirOrPrefix, string suffix)
        {
            WriteCustom(message, dirOrPrefix, suffix, true);
        }

        /// <summary>
        /// д���Զ�����־���Զ���Ŀ¼,��������Ӧ��Nlog.config����ʾ����
        ///  &lt;targets>
        ///    &lt;target name="LogCustom" xsi:type="File" layout="${message}"
        ///          fileName="${logDirectory}\${event-context:DirOrPrefix}${date:format=yyyyMMddHH}${event-context:Suffix}.txt">&lt;/target>
        ///  &lt;/targets>
        ///  &lt;rules>
        ///    &lt;logger name="LogCustom" level="Warn" writeTo="LogCustom" />
        /// </summary>
        /// <param name="message">Ҫд�����Ϣ</param>
        /// <param name="dirOrPrefix">
        /// д�뵽����Ŀ¼���ļ�ǰ׺������ַ�������\��������Ŀ¼
        /// ���� aa\bb ��д����ļ���ΪaaĿ¼�µ�bb��ͷ������
        /// </param>
        /// <param name="suffix">д�뵽���ļ���׺</param>
        /// <param name="addIpUrl">�Ƿ�Ҫ����ip��url����Ϣ</param>
        public static void WriteCustom(string message, string dirOrPrefix, string suffix, bool addIpUrl)
        {
            if (addIpUrl)
                message = BuildMessage(message);
            Logger logger1 = LogManager.GetLogger("LogCustom");
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, logger1.Name, message);
            logEvent.Properties["DirOrPrefix"] = dirOrPrefix;
            if (suffix != null)
                logEvent.Properties["Suffix"] = suffix;
            logger1.Log(logEvent);
        }
    }
}
