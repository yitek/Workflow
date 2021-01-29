using WFlow.Graphs;

namespace WFlow.Repositories
{
    public interface IGraphRepository
    {
        Node Fetch(string fullname, string version = null);
        Node Load(string fullname, string version);
    }
}