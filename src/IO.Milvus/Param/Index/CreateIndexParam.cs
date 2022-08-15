using IO.Milvus.Exception;
using System.Collections.Generic;

namespace IO.Milvus.Param.Index
{
    public class CreateIndexParam
    {
        private IndexType indexType = IndexType.INVALID;
        private MetricType metricType = MetricType.INVALID;
        private string extraParam;

        public static CreateIndexParam Create(
                    string collectionName,
                    string fieldName,
                    IndexType indexType,
                    MetricType metricType,
                    string indexName = "")
        {
            var param = new CreateIndexParam()
            {
                CollectionName = collectionName,
                FieldName = fieldName,
                IndexType = indexType,
                MetricType = metricType,
                IndexName = indexName
            };
            param.Check();

            return param;
        }

        public string CollectionName { get; set; }

        public string FieldName { get; set; }

        public string ExtraParam
        {
            get => extraParam; set
            {
                extraParam = value;
                ExtraDic[Constant.PARAMS] = extraParam;
            }
        }

        public Dictionary<string, string> ExtraDic { get; } = new Dictionary<string, string>();

        public IndexType IndexType
        {
            get => indexType; set
            {
                indexType = value;
                ExtraDic[Constant.INDEX_TYPE] = indexType.ToString();
            }
        }
        public MetricType MetricType
        {
            get => metricType; set
            {
                metricType = value;
                ExtraDic[Constant.METRIC_TYPE] = metricType.ToString();
            }
        }

        public string IndexName { get; set; }

        internal void Check()
        {
            ParamUtils.CheckNullEmptyString(CollectionName, "Collection name");
            ParamUtils.CheckNullEmptyString(FieldName, "Field name");            

            if (IndexType == IndexType.INVALID)
            {
                throw new ParamException("Index type is required");
            }

            if (MetricType == MetricType.INVALID)
            {
                throw new ParamException("Metric type is required");
            }
        }
    }
}
