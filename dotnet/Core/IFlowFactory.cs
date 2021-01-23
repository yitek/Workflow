using Flow.Graphs;

namespace Flow
{
    public interface IFlowFactory
    {
        Node Fetch(string fullname, string version = null);
        Node Load(string fullname, string version);
    }
}