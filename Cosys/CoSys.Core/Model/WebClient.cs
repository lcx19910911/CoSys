using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using CoSys.Model;

namespace CoSys.Core
{
    public class WebClient
    {
        public WebClient(HttpContextBase HttpContextBase)
        {
            this.HttpContext = HttpContextBase;
        }
        public HttpContextBase HttpContext { get; set; }

        public HttpRequestBase Request
        {
            get
            {
                return HttpContext.Request;
            }

        }

        public HttpSessionStateBase Session
        {
            get
            {
                return HttpContext.Session;
            }
        }


        private User _loginUser = null;

        public User LoginUser
        {
            get
            {
                return _loginUser != null ? _loginUser : LoginHelper.GetCurrentUser();
            }
            set
            {
                Session[Params.UserCookieName] = value;
            }
        }

        private Admin _loginAdmin = null;

        public Admin LoginAdmin
        {
            get
            {
                return _loginAdmin != null ? _loginAdmin : LoginHelper.GetCurrentAdmin();
            }
            set
            {
                Session[Params.AdminCookieName] = value;
            }
        }
        private string _postData = null;

        /// <summary>
        /// Post数据流
        /// </summary>
        public string PostData
        {
            get
            {
                if (_postData == null)
                {
                    var bytes = new byte[Request.InputStream.Length];
                    Request.InputStream.Read(bytes, 0, bytes.Length);
                    _postData = Encoding.Default.GetString(bytes);
                }
                return _postData;
            }
        }

        /// <summary>
        /// 获取上传的字符串值
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public string GetParam(string paramName)
        {
            return Request.Params[paramName];
        }

        /// <summary>
        /// 获取传值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public T GetParam<T>(string paramName) where T : class
        {
            return Request[paramName] as T;
        }



        #region 全局参数
        private string ip = null;
        /// <summary>
        /// 当前IP
        /// </summary>
        public string IP
        {
            get
            {
                if (ip.IsNullOrEmpty())
                {
                    ip = Request.UserHostAddress;
                    try
                    {
                        if (!ip.IsNullOrEmpty() && ip.StartsWith("10.", StringComparison.Ordinal))
                        {
                            ip = Request.ServerVariables["HTTP_X_REAL_IP"].Split(',')[0].Trim();
                        }
                    }
                    catch { }
                }
                return ip;
            }
        }

        private int _pageIndex = 0;

        /// <summary>
        /// 页面索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                if (_pageIndex == 0)
                {
                    _pageIndex = GetParam("PageIndex").ToInt32(0);
                    if (_pageIndex <= 0)
                    {
                        _pageIndex = 1;
                    }
                }
                return _pageIndex;
            }
        }

        private int _pageSize = 0;


        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize
        {
            get
            {
                if (_pageSize == 0)
                {
                    _pageSize = GetParam("PageSize").ToInt32(0);
                    if (_pageSize <= 0)
                    {
                        _pageSize = 12;
                    }
                }
                return _pageSize;
            }
        }


        /// <summary>
        /// 获取起始位置 (this.PageIndex - 1) * this.PageSize;
        /// </summary>
        public int Page_Skip
        {
            get
            {
                return (this.PageIndex - 1) * this.PageSize;
            }
        }


        #endregion
    }
}
