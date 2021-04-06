using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Study.Test
{
    public interface IElasticData
    {

    }

    //[ElasticIndex("ttmall_index_test", "product_test", "product_search_alias_test")]
    [ElasticsearchType(IdProperty = "Id")]
    public class ElasticSearchTestData : IElasticData
    {
        [Number(Name = "id")]
        public int Id { get; set; }

        [Keyword(Name = "name")]
        public string Name { get; set; }

        [Number(NumberType.Integer, Name = "age")]
        public int Age { get; set; }

        [Number(Ignore = true)]
        public int Type
        {
            get
            {

                return this.Age > 100 ? 1 : 0;
            }
        }
        //[Keyword(Ignore = true)]
        public string Reminder
        {
            get
            {

                return $"test{this.Name}";
            }
        }
    }
}
