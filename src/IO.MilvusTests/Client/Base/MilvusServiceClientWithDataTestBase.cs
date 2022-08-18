using Microsoft.VisualStudio.TestTools.UnitTesting;
using IO.Milvus.Param.Collection;
using IO.Milvus.Param.Dml;
using IO.Milvus.Param.Partition;
using IO.Milvus.Grpc;

namespace IO.MilvusTests.Client.Base
{
    public abstract class MilvusServiceClientWithDataTestBase
        : MilvusServiceClientTestsBase
    {

        #region Protected Methods
        protected InsertParam PreareData(string collectionName, string partitionName)
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

        protected void InsertData(string collectionName, string partitionName)
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

        protected void CreateCollection(string collectionName)
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