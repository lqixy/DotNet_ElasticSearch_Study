using Elasticsearch.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearch.Study.Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string INDEX_NAME = "ttmall_index";
        private const string RELATION_NAME = "my_relation";
        private readonly ElasticClient client;
        private const string SORT_FIELD = "sort";

        public UnitTest1()
        {
            var local = "http://192.168.0.31:9200/";
            var online = "http://8.140.136.122:9200/";
            var uris = new List<Uri> { new System.Uri(local) };
            var pool = new StaticConnectionPool(uris);
            var settings = new ConnectionSettings(pool);
            settings.DefaultIndex(INDEX_NAME);
            settings.DefaultMappingFor<ElasticSearchTestData>(m => m.RelationName(RELATION_NAME));
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
        public void IndexMany_Test()
        {
            var strs = CONTENT.Split("\r\n");
            var persons = Enumerable.Range(1, 100).Select(x =>
            {
                var seed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
                //var remiderIndex = GetIndex(usedIndex);
                //var nameIndex = GetIndex(usedIndex);
                var random = new Random(seed).Next(0, 3);
                var has = Enumerable.Range(0, random)
                    .Select(h => h);
                return new ElasticSearchTestData
                {
                    Id = x,
                    Name = strs[x],
                    Age = x * 10,
                    Reminder = strs[200 - x],
                    Sort = x,
                    Has = has ?? new int[0]
                };
            });

            var result = client.IndexMany(persons);
        }

        private int GetIndex(List<int> usedIndex)
        {
            var seed = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            var index = new Random(seed).Next(0, 100);
            if (!usedIndex.Any(i => i == index))
            {
                usedIndex.Add(index);
            }
            else
            {
                index = GetIndex(usedIndex);
            }

            return index;
        }

        [TestMethod]
        public async Task Index_Test()
        {
            var document = new ElasticSearchTestData
            {
                Id = 1,
                Name = $"������{1}",
                Age = 1 * 10
            };

            var result = await client.IndexDocumentAsync<ElasticSearchTestData>(document);
        }

        [TestMethod]
        public void Update_Test()
        {
            var list = new List<ElasticSearchTestData>
            {
                 new ElasticSearchTestData { Id = 4, Name = "�⻹��һ���޸�4" },
                 new ElasticSearchTestData { Id = 3, Name = "�⻹��һ���޸�3" }
            };

            client.IndexMany<ElasticSearchTestData>(list);
            //client.Update<ElasticSearchTestData>(updateData, x => x.Doc(updateData));
        }

        [TestMethod]
        public void Delete_Test()
        {
            var emptyList = new List<Guid>();
            var list = Enumerable.Empty<Guid>();
            var flag = Enumerable.Empty<Guid>().Equals(emptyList);
            //var result = client.DeleteByQuery<ElasticSearchTestData>(x => x.Query(q => q.Term(t => t.Id, 1)));
            var persons = Enumerable.Range(1, 3).Select(x => new ElasticSearchTestData
            {
                Id = x,
                //Name = $"������{x}",
                //Age = x * 10
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
            var ids = new int[] { 1, 20, 30, 40 };
            var sourceFieldStrs = new string[2] { "id", "name" };
            var mustQuerys = new List<Func<QueryContainerDescriptor<ElasticSearchTestData>, QueryContainer>>();

            mustQuerys.Add(x => x.Match(m => m.Field(f => f.Has).Query("1")));

            //// idС��5
            //mustQuerys.Add(x => x.TermRange(tr => tr.Field(f => f.Id).LessThan("5")));

            //// ��ֵ
            //mustQuerys.Add(x => x.Terms(t => t.Field(f => f.Id).Terms(ids)));
            //mustQuerys.Add(x => x.Terms(t => t.Field(f => f.Has).Terms(new int[1] { 0 })));

            //// ��������
            //mustQuerys.Add(x =>
            //        x.Match(t => t.Field(f => f.Name).Query("����"))
            //    || x.Match(m => m.Field(f => f.Reminder).Query("����")));


            //var matchQuerys = new List<Func<QueryContainerDescriptor<ElasticSearchTestData>, QueryContainer>>();
            //matchQuerys.Add(x => x.Match(m => m.Field(f => f.Name).Query("����")));


            // ����
            var sort = new SortDescriptor<ElasticSearchTestData>();
            sort.Descending(SORT_FIELD);

            // �����ֶ�
            var source = new SourceFilterDescriptor<ElasticSearchTestData>();
            //source.Includes(i => i.Fields(sourceFieldStrs));

            search
                .Query(q =>
                    q.Bool(b => b.Must(mustQuerys)
                    ))
                .Sort(s => sort)
                .Source(s => source)
                .From(0)
                    .Size(50);

            //search.Query(q => q.Bool(b => b.Must(m => m)));

            //search.Query(q => q
            //    .Match(m => m.Field(f => f.Name).Query("test")));


            var result = client.Search<ElasticSearchTestData>(search);
        }

        [TestMethod]
        public void Delete_Index_Test()
        {
            var existsResponse = client.Indices.Exists(INDEX_NAME);
            client.Indices.Delete(INDEX_NAME);
            //var index = client.Indices.Get(INDEX_NAME);
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
                //           //����������
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

        private const string CONTENT = @"Ǯ��������
��ʯ��ʱ��
��������ѧ
����̩��ϵ
����������
����̫����
�ϲ���̨��
���ǹ�����
�����ն���
����ʥ����
�������ɽ�
�������Ⱥ�
ˮ����Ƥʪ
�Ǵ�Ľ���
�������ѧ
��ţ������
���̻���
�ưͳԻ���
���к�����
����ε���
������Ĩ��
������ս��
�������㴬
�ƺ�������
��ѧ�о���
�����Ǳ���
�����Ķٺ�
֪��Ī����
�ٱ�ʤ֮ȯ
����ĸ���
���ʶ����
ר�ü����
���ܶ�����
���ӷ�����
���Ĵ�����
��������·
���Ϲ�����
��®ɽ�ٲ�
���������
������Ħ��
�޷�������
���׿˺�Ͽ
��е������
��ݺ���ľ
����ֹɽ��
��ȸ�ε���
�����󿳵�
�����в���
���������
�й����걨
����������
��Ʒ��Ӽ�
��������Դ
˼������λ
�����Ǹ�ԭ
����������
������ת��
��¿��ѩ��
��������˵
ս�Ժ�����
ץͷ����β
��������ѧ
��ʮ������
����������
Ⱥ��������
��ʮ�ķ���
�տ�˵�׻�
����������
���ϲ�����
��������ϵ
����������
������ͷ��
̫ƽ������
���������
��������˵
��½������
û���κδ�
����ʮ����
���������
������´�
���ƶ�����
���׶��ѧ
���ڼӹ���
����������
������Ⱥ��
��˿������
��������԰
û�ɲ��ɻ�
��ũ������
���ʲ�����
�ر�������
�����Ǻ���
���������
�ϴ����˹
��Ƥ������
����»�
�������
ԵԵ�����
��һ������
��ϼԪ����
ʣ���ֵ��
��֪������
ս����ɳ�
�����տ���
ʢ�겻����
Ħ����˹��
���ߵ��˶�
���ʻ�����
���ű�����
���ŵ���ѧ
�¹�Ӣ��˧
���ܶ�Ⱥ��
Ъ˹������
�������¼�
�������Ȫ
���������
���Ŷ�ʮƷ
�������͵�
����������
�����򲻴�
����������
����������
ο����ʤ��
��ͷ���˽�
Ƽ�������
��ϵ�̳�Ȩ
���ݴ�����
���Ͻ��ˮ
һ�ｵһ��
���������
���ĸ���Ƥ
�ʹ����¼�
���������
����ʳΪ��
��һ���±�
����������
����«��ư
���Ͳ�Τ
�������쳧
һ��һ����
Ĩһ���ӻ�
����������
������ֵ˰
����������
��������ַ
�����ϱ���
����������
��·������
���˲�ϧ��
������ҹ��
Ħ˹Ӳ�ȼ�
⨺�����ţ
�������뷨
ͬλ�ػ�ѧ
ϰ�߳���Ȼ
ͷ��������
���챤ս��
С�����к�
һ��ͬ����
̸Ц�к���
�������̨
�����ҵ�ò
�ֺ�������
�����ִ���
��¿������
ְҵ����ѧ
�������ս�
�Ͷ���ɽ��
��Ʒ������
������ľ��
���ܼ۸��
����Ĺ���
���ľͬ��
������ս��
����ʽ�ɻ�
���쵴֮ս
�������߲�
�Ŵ������
һ�����±�
һ�����G��
������ϯ��
���������
������˹̹
���²���Ȩ
�⽨ʿ���
���ָʾ��
��֮������
һ����֮��
���ȱȰ���
�з�ͷ����
����������
ʮ���I���
κ�깲�͹�
֤ȯ������
ͬ����ЧӦ
��ʥ������
��˼������
˾�ղ���Ϳ
�ƺ�������
����������";

    }

}
