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

        [Text(Name = "name")]
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

        [Keyword]
        public string TenantId { get; set; }

        [Keyword]
        public IEnumerable<string> TagIds { get; set; }

        //[Keyword(Ignore = true)]
        //public string Reminder
        //{
        //    get
        //    {

        //        return $"test{this.Name}";
        //    }
        //}
        public string Reminder { get; set; }
        public int Sort { get; set; }
        public IEnumerable<int> Has { get; set; }

        [Keyword]
        public string SpecpropId { get; set; }
        public int AdsId { get; set; }
    }

}
