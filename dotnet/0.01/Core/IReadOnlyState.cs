using System;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public interface IReadOnlyState
    {
        string this[string key] { get; }

        T State<T>(string key);
    }
}
