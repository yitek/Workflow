using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WFlow
{
    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current => throw new Exception("该属性不应该被调用");

        object IEnumerator.Current => this.Current;

        public void Dispose()
        {
           
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            
        }
    }
}
