using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearch.Study.Test
{
    public class PinyinFullFilter : PinyinFilterBase
    {
        public PinyinFullFilter() : this("pinyin")
        {
        }

        public PinyinFullFilter(string type, string firstLetter = "prefix", string paddingChar = " ") : base(type)
        {
            FirstLetter = firstLetter;
            PaddingChar = paddingChar;
        }

        [JsonProperty(PropertyName = "first_letter")]
        public override string FirstLetter { get; set; }

        [JsonProperty(PropertyName = "keep_first_letter")]
        public override bool KeepFirstLetter { get; set; } = false;

        [JsonProperty(PropertyName = "keep_full_pinyin")]
        public override bool KeepFullPinyin { get; set; } = true;

        [JsonProperty(PropertyName = "none_chinese_pinyin_tokenize")]
        public override bool NoneChinesePinyinTokenize { get; set; } = true;

        [JsonProperty(PropertyName = "padding_char")]
        public override string PaddingChar { get; set; }

    }
    public class PinyinFilterBase : TokenFilterBase, IPinyinFilter
    {
        public PinyinFilterBase(string type) : base(type)
        {
        }


        [JsonProperty(PropertyName = "first_letter")]
        public virtual string FirstLetter { get; set; } = "prefix";

        [JsonProperty(PropertyName = "keep_first_letter")]
        public virtual bool KeepFirstLetter { get; set; } = true;

        [JsonProperty(PropertyName = "keep_full_pinyin")]
        public virtual bool KeepFullPinyin { get; set; } = true;

        [JsonProperty(PropertyName = "keep_original")]
        public virtual bool KeepOriginal { get; set; } = false;

        [JsonProperty(PropertyName = "keep_separate_first_letter")]
        public virtual bool KeepSeparateFirstLetter { get; set; } = false;

        [JsonProperty(PropertyName = "limit_first_letter_length")]
        public virtual int LimitFirstLetterLength { get; set; } = 50;

        [JsonProperty(PropertyName = "lowercase")]
        public virtual bool Lowercase { get; set; } = true;

        [JsonProperty(PropertyName = "none_chinese_pinyin_tokenize")]
        public virtual bool NoneChinesePinyinTokenize { get; set; } = false;

        [JsonProperty(PropertyName = "padding_char")]
        public virtual string PaddingChar { get; set; } = "";
    }

    public interface IPinyinFilter : ITokenFilter
    {
        string FirstLetter { get; set; }

        bool KeepFirstLetter { get; set; }

        bool KeepSeparateFirstLetter { get; set; }

        bool KeepFullPinyin { get; set; }

        bool NoneChinesePinyinTokenize { get; set; }

        int LimitFirstLetterLength { get; set; }

        bool KeepOriginal { get; set; }

        bool Lowercase { get; set; }
    }
}
