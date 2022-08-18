using IO.Milvus.Param.Dml;
using IO.MilvusTests;
using IO.MilvusTests.Client.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Metadata.Ecma335;

namespace IO.Milvus.Client.Tests
{
    //TODO: Add search tests flow
    [TestClass]
    public class SearchTests : MilvusServiceClientWithDataTestBase
    {
        public const string CollectionName = nameof(SearchTests);

        [DataRow(CollectionName,HostConfig.DefaultTestPartitionName)]
        public void ATest(string collectionName,string partitionName)
        {
            //Prepare search collection
            CreateCollection(collectionName);
            InsertData(collectionName, partitionName);

            //Prepare Search parameter
            int searchK = 2;
            string searchParam = "{\"nprobe\":10}";

            //var searchParam = SearchParam<>
        }
    }

}
