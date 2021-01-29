using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WFlow.Entities;
using WFlow.Graphs;
using WFlow.Repositories;

namespace WFlow
{
    public interface IActivity
    {
        Guid Id { get; }
        Guid FlowId { get; }

        Guid? ParentId { get; }

        string NodeName { get; }
        string NodePath { get; }

        ActivityStates Status { get; }
        /// <summary>
        /// 是否已经处于终态
        /// </summary>
        bool IsFulfilled { get; }
        string Version { get; }

        
        string this[string key] { get; }

        IUser Closer { get; }
        IReadOnlyList<IActivity> Children { get; }
        DateTime? CloseTime { get; }
        DateTime CreateTime { get; }
        IUser Creator { get; }
        IUser Owner { get; }
        IUser Dealer { get; }
        DateTime? DealTime { get; }
        DateTime? DoneTime { get; }
       
        INode Graph { get; }
        
        IReadOnlyDictionary<string, Guid?> Nexts { get; }

       
        IActivity Parent { get; }
        
        
        

        T State<T>(string key);

        object State(string key);

        string ToString();
    }
}