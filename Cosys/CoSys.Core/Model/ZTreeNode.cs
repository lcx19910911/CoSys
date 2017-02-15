using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    /// <summary>
    /// ZTree树形模型
    /// </summary>
    public class ZTreeNode
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 节点Value值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 节点ischeck值
        /// </summary>
        public bool ischeck { get; set; }
        /// <summary>
        /// 节点nocheck值
        /// </summary>
        public bool nocheck { get; set; }        
        /// <summary>
        /// 子节点
        /// </summary>
        public List<ZTreeNode> children { get; set; }


        /// <summary>
        /// 节点类型
        /// </summary>
        public object nodeType { get; set; }
        
    }
}
