using WFlow.Graphs;

namespace WFlow.Repositories
{
    public interface IGraphRepository
    {
        INode Fetch(string fullname, string version = null);
        INode Load(string fullname, string version);
    }
}