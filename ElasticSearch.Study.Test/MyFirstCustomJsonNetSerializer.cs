using Elasticsearch.Net;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Study.Test
{
    public class MyFirstCustomJsonNetSerializer : ConnectionSettingsAwareSerializerBase
    {
        public MyFirstCustomJsonNetSerializer(IElasticsearchSerializer builtinSerializer
            , IConnectionSettingsValues connectionSettings)
            : base(builtinSerializer, connectionSettings)
        {
        }

        protected override JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include };
        }

        protected override void ModifyContractResolver(ConnectionSettingsAwareContractResolver resolver)
        {
            resolver.NamingStrategy = new SnakeCaseNamingStrategy();
        }
    }
}
