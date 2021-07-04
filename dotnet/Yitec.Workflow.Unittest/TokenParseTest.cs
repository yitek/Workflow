using NUnit.Framework;
using System.Threading.Tasks;
using Yitec.Workflow.Tokens;

namespace Yitec.Workflow.Unittest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Simple()
        {
            var content = "name:yiy\ngender:male\n";
            var token =await Token.ParseAsync(content);
            Assert.NotNull(token,"解析出来不能为空");
            Assert.NotNull(token["name"],"可以获取name");
            Assert.AreEqual("yiy",token["name"].ToString(),"name==yiy");
            Assert.NotNull(token["gender"], "可以获取ngender");
            Assert.AreEqual("male", token["gender"].ToString(), "ngender==male");
            Assert.Pass();
        }
        [Test]
        public async Task DeepTree()
        {
            var content = "name:yiy\nimports:\n\tname:yanyi\n\tgender:inner\ngender:outer\n";
            //content = "name:yiy\nimports:xxx\n";
           // content = "name:yiy\ngender:male\n";
            var token = await Token.ParseAsync(content);
            Assert.NotNull(token, "解析出来不能为空");
            var imports = token["imports"];
            Assert.NotNull(imports, "可以获取imports");
            Assert.IsAssignableFrom<ObjectToken>(imports, "imports是ObjectToken");
            Assert.AreEqual("yanyi",imports["name"].ToString(), "imports.name==yanyi");
            Assert.AreEqual("inner", imports["gender"].ToString(), "imports.gender==inner");
            Assert.AreEqual("outer", token["gender"].ToString(), "gender==outer");
            Assert.Pass();
        }
    }
}