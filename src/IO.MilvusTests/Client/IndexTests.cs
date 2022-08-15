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
    public class IndexTests: MilvusServiceClientTestsBase
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

        #region Private Methods
        private InsertParam PreareData(string collectionName, string partitionName)
        {
            var r = new Random(DateTime.Now.Second);

            var bookIds = new List<long>();
            var wordCounts = new List<long>();
            var bookIntros = new List<List<float>>();

            for (int i = 0; i < 2000; i++)
            {
                bookIds.Add(i);
                wordCounts.Add(i + 10000);
                var vector = new List<float>();
                for (int k = 0; k < 2; ++k)
                {
                    vector.Add(r.NextSingle());
                }
                bookIntros.Add(vector);
            }

            var insertParam = InsertParam.Create(collectionName, partitionName,
                new List<Field>()
                {
                    Field.Create(nameof(bookIds),bookIds),
                    Field.Create(nameof(wordCounts),wordCounts),
                    Field.CreateBinaryVectors(nameof(bookIntros),bookIntros),
                });

            return insertParam;
        }

        private void InsertData(string collectionName, string partitionName)
        {
            var data = PreareData(collectionName, partitionName);

            var hasP = MilvusClient.HasPartition(HasPartitionParam.Create(collectionName, partitionName));
            if (!hasP.Data)
            {
                var createP = MilvusClient.CreatePartition(CreatePartitionParam.Create(collectionName, partitionName));
                AssertRpcStatus(createP);
            }

            var r = MilvusClient.Insert(data);

            Assert.IsNotNull(r);
            Assert.IsTrue(r.Status == Milvus.Param.Status.Success, r.Exception?.ToString());
            Assert.IsTrue(r.Data.SuccIndex.Count > 0);
        }

        private void CreateCollection(string collectionName)
        {
            var hasBookCollection = MilvusClient.HasCollection(collectionName);
            if (!hasBookCollection.Data)
            {
                var r = MilvusClient.CreateCollection(
                    CreateCollectionParam.Create(
                        collectionName,
                        2,
                        new List<FieldType>()
                        {
                            FieldType.Create(
                                "bookIds",
                                DataType.Int64,
                                isPrimaryLey:true),
                            FieldType.Create("wordCounts",DataType.Int64),
                            FieldType.Create(
                                "bookIntros",
                                "",
                                DataType.FloatVector,
                                dimension:2,
                                0)
                        }));

                AssertRpcStatus(r);
            }
        }
        #endregion
    }
}
