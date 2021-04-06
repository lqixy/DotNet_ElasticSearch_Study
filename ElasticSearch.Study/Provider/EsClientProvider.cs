using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Study.Provider
{
    public class EsClientProvider : IEsClientProvider
    {
        private readonly IElasticsearchConfiguration elasticsearchConfiguration;
        private ElasticClient client;
        private readonly IConnectionPool connectionPool;
        public EsClientProvider(IElasticsearchConfiguration elasticsearchConfiguration)
        {
            var uris = new List<Uri> { elasticsearchConfiguration.ElasticUri };
            connectionPool = new StaticConnectionPool(uris);

            var settings = new ConnectionSettings(connectionPool);
            settings.DefaultFieldNameInferrer((name) => name);
            settings.DefaultIndex(elasticsearchConfiguration.DefaultIndexName);

            client = new ElasticClient(settings);
            this.elasticsearchConfiguration = elasticsearchConfiguration;

        }

        public ElasticClient GetClient()
        {
            //InitClient();
            return client;
        }

        //public void InitClient()
        //{
        //    var uri = new Uri(configuration["EsUrl"]);
        //    client = new ElasticClient(uri);
        //}
    }
}
