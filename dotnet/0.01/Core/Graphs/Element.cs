using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow.Graphs
{
    public class Element : IElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示用名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 实例名称，运行期会实例化该类型，并调用execute
        /// </summary>
        public string InstanceType { get; set; }


        public string Key { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// 配置的数据，会自动内化到states中
        /// </summary>

        public JObject Meta { get; set; }

        
    }
}
