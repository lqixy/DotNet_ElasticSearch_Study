using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Study.Provider
{
    public interface IEsClientProvider
    {
        ElasticClient GetClient();
    }
}
