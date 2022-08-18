using IO.Milvus.Grpc;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Dml;
using IO.Milvus.Param.Index;
using IO.Milvus.Param.Partition;
using IO.MilvusTests;
using IO.MilvusTests.Client.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IO.Milvus.Client.Tests
{
    /// <summary>
    /// Tests for index
    /// <see href="https://milvus.io/docs/v2.1.x/drop_index.md"/>
    /// </summary>
    [TestClass]
    public class IndexTests: MilvusServiceClientWithDataTestBase
    {
        public const string CollectionName = nameof(IndexTests);

        [TestMethod]
        [DataRow(CollectionName,HostConfig.DefaultTestPartitionName)]
        public void ABuildIndexTest(string collectionName,string partitionName)
        {
            CreateCollection(collectionName);

            InsertData(collectionName, partitionName);

            var indexParam = CreateIndexParam.Create(
                collectionName: collectionName,
                fieldName: "bookIntros",
                indexType: Milvus.Param.IndexType.IVF_FLAT,
                metricType: Milvus.Param.MetricType.L2);
            indexParam.ExtraParam = "{\"nlist\":1024}";
            
            var r = MilvusClient.CreateIndex(
                indexParam);

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Milvus.Param.Status.Success, r.Exception?.ToString());
        }

        [TestMethod]
        [DataRow(CollectionName)]
        public void BDropIndexTest(string collectionName)
        {
            var r = MilvusClient.DropIndex(DropIndexParam.Create(collectionName, "bookIntros"));

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Milvus.Param.Status.Success, r.Exception?.ToString());
        }

        [TestMethod]
        [DataRow(CollectionName)]
        public void CDeleteCollection(string collectionName)
        {
            var r = MilvusClient.DropCollection(collectionName);

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Param.Status.Success, r.Exception?.ToString());
        }
    }
}
