namespace IO.Milvus.Param.Index
{
    public class DropIndexParam
    {
        public static DropIndexParam Create(
            string collectionName,
            string fieldName,
            string indexName = "")
        {
            var param = new DropIndexParam()
            {
                CollectionName = collectionName,
                IndexName = indexName,
                FieldName = fieldName
            };
            param.Check();

            return param;
        }

        public string CollectionName { get; set; }

        public string IndexName { get; set; } = Constant.DEFAULT_INDEX_NAME;

        public string FieldName { get; set; }

        internal void Check()
        {
            if (string.IsNullOrEmpty(IndexName))
            {
                IndexName = Constant.DEFAULT_INDEX_NAME;
            }
            ParamUtils.CheckNullEmptyString(FieldName, "Field Name");
        }
    }
}
