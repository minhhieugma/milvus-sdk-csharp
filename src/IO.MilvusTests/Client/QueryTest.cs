using IO.Milvus.Param.Dml;
using IO.MilvusTests.Client.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IO.Milvus.Client.Tests
{
    //TODO:Add base testmethods so that it can be executed without pre-import data.

    [TestClass]
    public class QueryTest : MilvusServiceClientTestsBase
    {
        [TestMethod()]
        [DataRow("book_id > 0 && book_id < 2000")]
        public void QueryDataTest(string expr)
        {
            var r = MilvusClient.Query(QueryParam.Create(
                "book",
                new List<string> { "_default" },
                new List<string>() { "book_id", "word_count", "book_intro" },
                expr: expr));

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Param.Status.Success);
        }
    }
}
