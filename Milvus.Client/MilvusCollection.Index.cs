namespace Milvus.Client;

public partial class MilvusCollection
{
    /// <summary>
    /// Creates an index.
    /// </summary>
    /// <param name="fieldName">The name of the field in the collection for which the index will be created.</param>
    /// <param name="indexType">The type of the index to be created.</param>
    /// <param name="metricType"></param>
    /// <param name="extraParams">
    /// Extra parameters specific to each index type; consult the documentation for your index type for more details.
    /// </param>
    /// <param name="indexName">An optional name for the index to be created.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task CreateIndexAsync(
        string fieldName,
        IndexType? indexType = null,
        SimilarityMetricType? metricType = null,
        IDictionary<string, string>? extraParams = null,
        string? indexName = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        var request = new CreateIndexRequest
        {
            CollectionName = Name,
            FieldName = fieldName
        };

        if (indexName is not null)
        {
            request.IndexName = indexName;
        }

        if (metricType is not null)
        {
            request.ExtraParams.Add(new Grpc.KeyValuePair
            {
                Key = "metric_type",
                Value = GetGrpcMetricType(metricType.Value)
            });
        }

        if (indexType is not null)
        {
            request.ExtraParams.Add(new Grpc.KeyValuePair
            {
                Key = "index_type",
                Value = GetGrpcIndexType(indexType.Value)
            });
        }

        if (extraParams is not null)
        {
            request.ExtraParams.Add(new Grpc.KeyValuePair
            {
                Key = "params",
                Value = Combine(extraParams)
            });
        }

        await _client.InvokeAsync(_client.GrpcClient.CreateIndexAsync, request, cancellationToken)
            .ConfigureAwait(false);

        static string GetGrpcIndexType(IndexType indexType)
            => indexType switch
            {
                IndexType.Invalid => "INVALID",
                IndexType.Flat => "FLAT",
                IndexType.IvfFlat => "IVF_FLAT",
                IndexType.IvfPq => "IVF_PQ",
                IndexType.IvfSq8 => "IVF_SQ8",
                IndexType.Hnsw => "HNSW",
                IndexType.RhnswFlat => "RHNSW_FLAT",
                IndexType.RhnswPq => "RHNSW_PQ",
                IndexType.RhnswSq => "RHNSW_SQ",
                IndexType.Annoy => "ANNOY",
                IndexType.BinFlat => "BIN_FLAT",
                IndexType.BinIvfFlat => "BIN_IVF_FLAT",
                IndexType.AutoIndex => "AUTOINDEX",

                _ => throw new ArgumentOutOfRangeException(nameof(indexType), indexType, null)
            };

        static string GetGrpcMetricType(SimilarityMetricType similarityMetricType)
            => similarityMetricType switch
            {
                SimilarityMetricType.Invalid => "INVALID",
                SimilarityMetricType.L2 => "L2",
                SimilarityMetricType.Ip => "IP",
                SimilarityMetricType.Jaccard => "JACCARD",
                SimilarityMetricType.Tanimoto => "TANIMOTO",
                SimilarityMetricType.Hamming => "HAMMING",
                SimilarityMetricType.Superstructure => "SUPERSTRUCTURE",
                SimilarityMetricType.Substructure => "SUBSTRUCTURE",

                _ => throw new ArgumentOutOfRangeException(nameof(similarityMetricType), similarityMetricType, null)
            };
    }

    /// <summary>
    /// Drops an index.
    /// </summary>
    /// <param name="fieldName">The name of the field which has the index to be dropped.</param>
    /// <param name="indexName">An optional name of the index to be dropped.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task DropIndexAsync(
        string fieldName, string? indexName = null, CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        var request = new DropIndexRequest { CollectionName = Name, FieldName = fieldName };

        if (indexName is not null)
        {
            request.IndexName = indexName;
        }

        await _client.InvokeAsync(_client.GrpcClient.DropIndexAsync, request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Describes an index, returning information about it's configuration.
    /// </summary>
    /// <param name="fieldName">The name of the field which has the index to be described.</param>
    /// <param name="indexName">An optional name of the index to be described.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task<IList<MilvusIndexInfo>> DescribeIndexAsync(
        string fieldName,
        string? indexName = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        var request = new DescribeIndexRequest { CollectionName = Name, FieldName = fieldName };

        if (indexName is not null)
        {
            request.IndexName = indexName;
        }

        DescribeIndexResponse response =
            await _client.InvokeAsync(_client.GrpcClient.DescribeIndexAsync, request, static r => r.Status,
                    cancellationToken)
                .ConfigureAwait(false);

        List<MilvusIndexInfo> indexes = new();
        if (response.IndexDescriptions is not null)
        {
            foreach (IndexDescription index in response.IndexDescriptions)
            {
                indexes.Add(new MilvusIndexInfo(
                    index.FieldName,
                    index.IndexName,
                    index.IndexID,
                    index.Params.ToDictionary(static p => p.Key, static p => p.Value)));
            }
        }

        return indexes;
    }

    /// <summary>
    /// Gets the state of an index.
    /// </summary>
    /// <param name="fieldName">The vector field name in this particular collection</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task<IndexState> GetIndexStateAsync(string fieldName, CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        var request = new GetIndexStateRequest { CollectionName = Name, FieldName = fieldName };

        GetIndexStateResponse response =
            await _client.InvokeAsync(_client.GrpcClient.GetIndexStateAsync, request, static r => r.Status,
                    cancellationToken)
                .ConfigureAwait(false);

        return response.State switch
        {
            Grpc.IndexState.None => IndexState.None,
            Grpc.IndexState.Unissued => IndexState.Unissued,
            Grpc.IndexState.InProgress => IndexState.InProgress,
            Grpc.IndexState.Finished => IndexState.Finished,
            Grpc.IndexState.Failed => IndexState.Failed,
            Grpc.IndexState.Retry => IndexState.Retry,

            _ => throw new InvalidOperationException($"Unknown {nameof(Grpc.IndexState)}: {response.State}")
        };
    }

    /// <summary>
    /// Gets the build progress of an index.
    /// </summary>
    /// <param name="fieldName">The name of the field which has the index.</param>
    /// <param name="indexName">An optional name of the index.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task<IndexBuildProgress> GetIndexBuildProgressAsync(
        string fieldName,
        string? indexName = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        var request = new GetIndexBuildProgressRequest { CollectionName = Name, FieldName = fieldName };

        if (indexName is not null)
        {
            request.IndexName = indexName;
        }

        GetIndexBuildProgressResponse response =
            await _client.InvokeAsync(_client.GrpcClient.GetIndexBuildProgressAsync, request, static r => r.Status,
                    cancellationToken)
                .ConfigureAwait(false);

        return new IndexBuildProgress(response.IndexedRows, response.TotalRows);
    }

    /// <summary>
    /// Polls Milvus for building progress of an index until it is fully built.
    /// To perform a single progress check, use <see cref="GetIndexBuildProgressAsync" />.
    /// </summary>
    /// <param name="fieldName">The vector field name in this particular collection</param>
    /// <param name="indexName">An optional name for the index to be created.</param>
    /// <param name="waitingInterval">Waiting interval. Defaults to 500 milliseconds.</param>
    /// <param name="timeout">How long to poll for before throwing a <see cref="TimeoutException" />.</param>
    /// <param name="progress">Provides information about the progress of the loading operation.</param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None" />.
    /// </param>
    public async Task WaitForIndexBuildAsync(
        string fieldName,
        string? indexName = null,
        TimeSpan? waitingInterval = null,
        TimeSpan? timeout = null,
        IProgress<IndexBuildProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        Verify.NotNullOrWhiteSpace(fieldName);

        await Utils.Poll(
            async () =>
            {
                IndexBuildProgress progress = await GetIndexBuildProgressAsync(fieldName, indexName, cancellationToken)
                    .ConfigureAwait(false);
                return (progress.IsComplete, progress);
            },
            indexName is null
                ? $"Timeout when waiting for index on collection '{Name}' to build"
                : $"Timeout when waiting for index '{indexName}' on collection '{Name}' to build",
            waitingInterval, timeout, progress, cancellationToken).ConfigureAwait(false);
    }
}
