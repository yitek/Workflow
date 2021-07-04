using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.Domains
{
    /// <summary>
    /// 处理者
    /// </summary>
    public interface IDealer
    {
        public Guid Id { get;  }
        public string Name { get; }
    }
}
