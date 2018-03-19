using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Analysis.Hunspell;

namespace NewsManage.Models
{
    public class TemplateData : Dictionary<string,Item>
    {
        public TemplateData()
        {
        }

        public TemplateData(string Key, Item Value)
        {
            key = Key;
            value = Value;
        }

        public string key { get; set; }

        public Item value { get; set; }

        

    }
}
