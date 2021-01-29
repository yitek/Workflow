//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;

//namespace WFlow.Graphs
//{
//    public class Digraph :  IDigraph
//    {
//        Node node;
//        Association association;
//        IDigraphable parentGraphable;

//        public Digraph(IDigraphable parentGraphable, IDigraphable me, Association association)
//        {
//            this.node = currentNode;
//            this.association = association;
//            this.parentGraphable = parentGraphable;
//        }
//        void initParentAndNexts() {
//            if (this.nexts != null) return;
//            this.parent = this.parentGraphable.Graph as Digraph;
//            var assocFromMe = this.parent.node.Associations.Where(a => a.From == this.node.Name);
//            foreach(var assoc)
//        }

//        Digraph parent;
//        /// <summary>
//        /// 上级图元
//        /// </summary>
//        public Digraph Parent { get { return parent; } }
//        List<Next> nexts;
//        public IReadOnlyList<Next> Nexts
//        {
//            get { return nexts; }
//        }

//        List<Digraph> children;
//        public IReadOnlyList<Digraph> Children
//        {
//            get { return children; }
//        }


//        public string AssociationName { get { return this.association.Name; } }

//        public string AssociationDisplayName { get { return this.association.DisplayName; } }

//        public IReadOnlyDictionary<string, string> AssociationMeta { get { return this.association.Meta; } }

        

//        public string Name { get { return this.node.Name; } }

//        public string DisplayName { get { return this.node.DisplayName; } }

//        public string ActivityType { get { return this.node.InstanceType; } }

//        public IReadOnlyDictionary<string, string> InParameters
//        {
//            get { return this.node.InParameters; }
//        }

//        public IReadOnlyDictionary<string, string> OutParameters
//        {
//            get { return this.node.OutParameters; }
//        }

//        public IReadOnlyDictionary<string, string> Meta
//        {
//            get { return this.node.Meta; }
//        }


//        public string this[string key]
//        {
//            get
//            {
//                if (this.node.Meta == null || this.node.Meta.Count == 0) return null;
//                this.node.Meta.TryGetValue(key, out string value);
//                return value;
//            }
//        }

//        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
//        {
//            if (this.node.Meta == null || this.node.Meta.Count == 0) return new EmptyEnumerator<KeyValuePair<string, string>>();

//            return this.node.Meta.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }
//    }
//}
