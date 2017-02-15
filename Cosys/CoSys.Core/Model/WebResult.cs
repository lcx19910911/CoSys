
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoSys.Core
{
    /// <summary>
    /// 返回结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebResult<T>
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public ErrorCode Code;

        /// <summary>
        /// 返回结果
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// 附加消息
        /// </summary>
        public string Append { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool OccurError
        {
            get { return Code != ErrorCode.sys_success; }
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get { return Code == ErrorCode.sys_success; }
        }
    }
}
