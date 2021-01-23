using Flow.Graphs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Flow
{
    public class FlowFactory : IFlowFactory
    {
        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Node>> caches;
        public FlowFactory()
        {
            this.caches = new ConcurrentDictionary<string, ConcurrentDictionary<string, Node>>();
        }
        public Node Fetch(string fullname, string version = null)
        {
            var versionCache = this.caches.GetOrAdd(version, (version) => new ConcurrentDictionary<string, Node>());
            return versionCache.GetOrAdd(fullname, (fname) => this.Load(fullname, version));
        }

        public Node Load(string fullname, string version)
        {
            if (version == null) version = "default";
            var basDir = AppDomain.CurrentDomain.BaseDirectory;
            var versionDir = Path.Combine(basDir, version);
            var def_filename = Path.Combine(versionDir, fullname) + ".json";
            var content = File.ReadAllText(def_filename);
            var node = JSON.Parse<Node>(content);

            return node;
        }

        public readonly static FlowFactory Default = new FlowFactory();
    }
}
