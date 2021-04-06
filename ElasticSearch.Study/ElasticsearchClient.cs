using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Study
{
    public class ElasticsearchConfiguration : IElasticsearchConfiguration
    {
        private readonly string ES_URL_KEY = "Elasticsearch.Url";
        private readonly string ES_INDEX_NAME_KEY = "Elasticsearch.IndexName";
        private readonly IConfiguration configuration;

        public ElasticsearchConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Uri ElasticUri
        {
            get
            {
                var uri = configuration[ES_URL_KEY];
                if (string.IsNullOrWhiteSpace(uri))
                {
                    uri = @"http://localhost:9200";
                }
                return new Uri(uri);
            }
        }

        public string DefaultIndexName
        {
            get
            {
                var indexName = configuration[ES_INDEX_NAME_KEY];
                if (string.IsNullOrWhiteSpace(indexName))
                {
                    indexName = "defaultindex";
                }
                return indexName;
            }
        }

        public int NumberOfReplicas => 1;

        public int NumberOfShards => 5;
    }
}
