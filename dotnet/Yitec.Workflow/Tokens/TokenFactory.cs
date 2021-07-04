using System;
using System.Collections.Generic;
using System.Linq;

namespace Yitec.Workflow.Tokens
{
    public class TokenFactory : ITokenFactory
    {
        public int CountArray(object array)
        {
            return (array as IList<Token>).Count;
        }

        public Token GetToken(object taret, string name)
        {
            throw new NotImplementedException();
        }

        public Token GetToken(object atarget, int index)
        {
            throw new NotImplementedException();
        }

        public Token ParseToken(string text)
        {
            throw new NotImplementedException();
        }

        public void SetToken(object target, string name, object value)
        {
            throw new NotImplementedException();
        }

        public void SetToken(object target, int index, object value)
        {
            throw new NotImplementedException();
        }

        public readonly static TokenFactory YMLFactory = new TokenFactory();
    }
}
