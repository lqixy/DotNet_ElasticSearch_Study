using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearch.Study.Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string INDEX_NAME = "test_index";
        private readonly ElasticClient client;
        public UnitTest1()
        {
            var local = "http://localhost:9200/";
            var online = "http://8.140.136.122:9200/";
            var uris = new List<Uri> { new System.Uri(local) };
            var pool = new StaticConnectionPool(uris);
            var settings = new ConnectionSettings(pool);
            settings.DefaultIndex(INDEX_NAME);

            //settings.

            client = new ElasticClient(settings);
        }

        [TestMethod]
        public void Create_Index_Test()
        {
            //client.create
            CreateIndex();
        }

        [TestMethod]
        public void Insert_Test()
        {
            var persons = Enumerable.Range(1, 10).Select(x => new ElasticSearchTestData
            {
                Id = x,
                Name = $"测试张{x}",
                Age = x * 10
            });

            var result = client.IndexMany(persons);
        }

        [TestMethod]
        public void Update_Test()
        {
            var result = client.Indices;
            client.UpdateByQuery<ElasticSearchTestData>(x => x.Query(q => 
                q.Term(t=>t.Id,4)));
        }

        [TestMethod]
        public void Delete_Test()
        {

            //var result = client.DeleteByQuery<ElasticSearchTestData>(x => x.Query(q => q.Term(t => t.Id, 1)));
            var persons = Enumerable.Range(1, 3).Select(x => new ElasticSearchTestData
            {
                Id = x,
                Name = $"测试张{x}",
                Age = x * 10
            });
            var result = client.DeleteMany(persons, INDEX_NAME);
        }

        [TestMethod]
        public void Query_Test()
        {
            var search = new SearchDescriptor<ElasticSearchTestData>();
            //search.Query(q => q.Bool(b => b
            //      .Must(m => m
            //          .Term(tm => tm
            //            .Field(f => f.Id).Value(2))
            //          )
            //      )
            //);

            search.Query(q => q
                .Match(m => m.Field(f => f.Name).Query("test")));


            var result = client.Search<ElasticSearchTestData>(search);
        }

        private void CreateIndex()
        {
            //var esResult = client.Indices.Exists(INDEX_NAME);
            if (!IndexExists())
            {
                IndexState indexState = new IndexState
                {
                    Settings = new IndexSettings
                    {
                        NumberOfReplicas = 1,
                        NumberOfShards = 5
                    }
                };

                var result = client.Indices.Create(INDEX_NAME);
                //var result = client.Indices.Create(INDEX_NAME, p => p
                //   .InitializeUsing(indexState).Settings(s => s.Analysis(
                //       an => an
                //       .TokenFilters(tf => tf.UserDefined("pinyin_quanpin_filter", new PinyinFullFilter())
                //       .EdgeNGram("edge_ngram_filter", eng => eng.MaxGram(50).MinGram(1)))
                //       //.CharFilters(cf=>cf.)
                //       .Analyzers(ana => ana
                //           //新增分析器
                //           .Custom("pinyin_analyzer", ca => ca
                //               .Tokenizer("keyword")
                //               .Filters("pinyin_quanpin_filter", "word_delimiter"))
                //           //.Custom("pinyin_analyzer", ca => ca
                //           //    .Tokenizer("keyword")
                //           //    .Filters("pinyin_quanpin_filter", "word_delimiter"))

                //           )
                //       ))
                //   .Aliases(a => a.Alias(INDEX_NAME))
                //   //.Mappings(m => m.Map<T>(typeof(T).Name, mps => mps.AutoMap()))
                //   .Map<ElasticSearchTestData>(map => map.AutoMap())
                //    );
            }
        }

        private bool IndexExists()
        {
            var exists = client.Indices.Exists(INDEX_NAME).Exists;
            return exists;
        }
    }

    //[ElasticsearchType(IdProperty = "id")]
    //public class Person
    //{
    //    [Number(Name = "id")]
    //    public int Id { get; set; }

    //    [Keyword(Name = "name")]
    //    public string Name { get; set; }

    //    [Number(Name = "age")]
    //    public int Age { get; set; }
    //}

    //public class ElasticPropery : ElasticsearchPropertyAttributeBase
    //{
    //    public ElasticPropery(FieldType type, string name) : base(type)
    //    {
    //    }
    //}
}
