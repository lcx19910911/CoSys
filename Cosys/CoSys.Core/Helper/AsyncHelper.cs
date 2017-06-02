using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoSys.Core
{
    /// <summary>
    /// 异步的
    /// </summary>
    public class AsyncHelper
    {

        public static void Run(Action<object> action, object state)
        {
            Task task = new Task(action, state);
            task.ContinueWith((t) =>
            {
                if (t.Exception != null)
                {
                    LogHelper.WriteException(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }

        public static void Run(Action action)
        {
            Task.Run(action).ContinueWith((t) =>
            {
                if (t.Exception != null)
                {
                    LogHelper.WriteException(t.Exception);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
