namespace Yitec.Workflow
{
    public interface ITokenFactory
    {
        Token GetToken(object taret, string name);
        void SetToken(object target, string name, object value);
        Token GetToken(object atarget, int index);
        void SetToken(object target, int index, object value);

        int CountArray(object array);
        Token ParseToken(string text);
        
    }
}