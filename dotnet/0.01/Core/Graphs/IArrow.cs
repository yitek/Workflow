namespace WFlow.Graphs
{
    public interface IArrow:IElement
    {
        string Key { get;  }
        string To { get;  }
        string Value { get;  }
    }
}