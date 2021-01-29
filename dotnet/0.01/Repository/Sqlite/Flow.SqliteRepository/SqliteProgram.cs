using System;

namespace WFlow.Repositories
{
    class SqliteProgram
    {
        static readonly string ConnectionString = "Data Source=wf.db;Version=3;New=True;Compress=True;BinaryGUID=False;";
        static void Main(string[] args)
        {
            Engine.Development = true;
            var arepo = new SqliteFlowRepository(ConnectionString);
            var factory = new Engine(null,arepo);
            var user = new User("01","yiy");
            factory.StartFlow("sample.wf",null,user,null);
        }
    }
}
