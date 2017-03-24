using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Model
{
    /// <summary>
    /// 选择项
    /// </summary>
    public class SelectItem
    {

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected { get; set; } = false;

        public string ParentKey { get; set; }

    }
}
