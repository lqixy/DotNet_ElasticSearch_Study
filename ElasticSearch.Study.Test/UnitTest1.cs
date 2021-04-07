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
                Name = $"测试张{1}",
                Age = 1 * 10
            };

            var result = await client.IndexDocumentAsync<ElasticSearchTestData>(document);
        }

        [TestMethod]
        public void Update_Test()
        {
            var list = new List<ElasticSearchTestData>
            {
                 new ElasticSearchTestData { Id = 4, Name = "这还是一个修改4" },
                 new ElasticSearchTestData { Id = 3, Name = "这还是一个修改3" }
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
                //Name = $"测试张{x}",
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

            //// id小于5
            //mustQuerys.Add(x => x.TermRange(tr => tr.Field(f => f.Id).LessThan("5")));

            //// 多值
            //mustQuerys.Add(x => x.Terms(t => t.Field(f => f.Id).Terms(ids)));
            //mustQuerys.Add(x => x.Terms(t => t.Field(f => f.Has).Terms(new int[1] { 0 })));

            //// 包含文字
            //mustQuerys.Add(x =>
            //        x.Match(t => t.Field(f => f.Name).Query("三角"))
            //    || x.Match(m => m.Field(f => f.Reminder).Query("电视")));


            //var matchQuerys = new List<Func<QueryContainerDescriptor<ElasticSearchTestData>, QueryContainer>>();
            //matchQuerys.Add(x => x.Match(m => m.Field(f => f.Name).Query("测试")));


            // 排序
            var sort = new SortDescriptor<ElasticSearchTestData>();
            sort.Descending(SORT_FIELD);

            // 返回字段
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

        private const string CONTENT = @"钱塘湖春行
新石器时代
分子生物学
阿尔泰语系
无政府主义
雁门太守行
上不得台盘
恒星光谱型
开普勒定律
雁塔圣教序
慢工出巧匠
密西西比河
水过地皮湿
那达慕大会
社会心理学
骑牛读汉书
广大教化主
哑巴吃黄连
地中海气候
赤爵衔丹书
往脸上抹黑
马拉松战役
稳坐钓鱼船
黄河三角洲
文学研究会
几内亚比绍
静静的顿河
知子莫若父
操必胜之券
概念的概括
身后识方干
专用计算机
哈密顿问题
电视发射塔
古文辞类纂
电气化铁路
联合国宪章
望庐山瀑布
马尔马拉海
鸯窭利摩罗
无风三尺浪
德雷克海峡
机械化步兵
蚍蜉撼大木
西高止山脉
赤雀衔丹书
麻扎大砍刀
有幸有不幸
脊髓灰质炎
中国青年报
麦琪的礼物
油品添加剂
非再生资源
思不出其位
圭亚那高原
三相四线制
货物周转量
骑驴风雪中
阴阳五行说
战略核武器
抓头不是尾
生物物理学
六十花甲子
福尔赛世家
群体凝聚力
二十四番风
空口说白话
莲花步步生
瞒上不瞒下
银道坐标系
五氧化二磷
花岗岩头脑
太平军北伐
喷射混凝土
黄生借书说
大陆性气候
没做奈何处
金钗十二行
国家与革命
商务办事处
南唐二主词
多伦多大学
出口加工区
无利不起早
夏威夷群岛
单丝不成线
玻璃动物园
没巧不成话
半农半牧区
国际博览会
特别行政区
反三角函数
波多黎各岛
毕达哥拉斯
瓜皮搭李树
真金不怕火
鹌鹑馉饳儿
缘缘堂随笔
二一添作五
碧霞元君祠
剩余价值率
不知何许人
战斗里成长
亚穆苏克罗
盛年不重来
摩尔曼斯克
无线电运动
国际互联网
黄门北寺狱
老张的哲学
穆桂英挂帅
马绍尔群岛
歇斯的里亚
匈牙利事件
华清池温泉
青年近卫军
龙门二十品
凝固汽油弹
安西都护府
欲速则不达
比勒陀利亚
着三不着两
慰情聊胜无
顾头不顾脚
萍浏醴起义
父系继承权
福州船政局
屋上建瓴水
一物降一物
义务兵役制
人心隔肚皮
皇姑屯事件
俭可以助廉
民以食为本
九一八事变
三杯和万事
依葫芦画瓢
大津巴布韦
汽车制造厂
一让一个肯
抹一鼻子灰
法定代表人
土地增值税
蜉蝣撼大树
盘龙城遗址
东西南北人
长江三角洲
半路里姻眷
武人不惜死
欢娱嫌夜短
摩斯硬度计
猕猴骑土牛
四舍五入法
同位素化学
习惯成自然
头三脚难踢
普天堡战斗
小番子闲汉
一寸同心缕
谈笑有鸿儒
中央电视台
花岗岩地貌
灾害性天气
军队现代化
倒驴不倒架
职业伦理学
官无三日紧
巴尔干山脉
九品中正制
特洛伊木马
性能价格比
象棋的故事
与草木同朽
反革命战争
活塞式飞机
黄天荡之战
根菜类蔬菜
脚搭着脑杓
一二八事变
一心无罣碍
门面铺席人
读书出版社
哈萨克斯坦
领事裁判权
封建士大夫
酸碱指示剂
爱之欲其生
一假手之劳
伊比比奥人
有发头陀寺
五氧化二钒
十三镮金带
魏玛共和国
证券交易所
同离子效应
至圣文宣王
三思而后行
司空不视涂
酒后吐真言
犯罪嫌疑人";

    }

}
