using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Study
{
    public interface IElasticsearchConfiguration
    {
        Uri ElasticUri { get; }

        string DefaultIndexName { get; }

        int NumberOfReplicas { get; }

        int NumberOfShards { get; }
    }
}
