using WFlow.Graphs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WFlow.Repositories
{
    public class GraphRepository : IGraphRepository
    {
        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Node>> caches;
        public string baseDir;
        public GraphRepository(string wf_graphCnnectionString)
        {
            this.baseDir = wf_graphCnnectionString;
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
            var basDir = this.baseDir;
            var versionDir = Path.Combine(basDir, version);
            var def_filename = Path.Combine(versionDir, fullname) + ".json";
            var content = File.ReadAllText(def_filename);
            var node = JSON.Parse<Node>(content);

            return node;
        }

        public static GraphRepository Default = new GraphRepository(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"wf_defs"));

    }
}
