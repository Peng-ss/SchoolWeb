using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManage.Models
{
    public class Item 
    {
        public Item(string value, string color)
        {
            this.value = value;
            this.color = color;
        }

        public string value { get; set; }
        public string color { get; set; }
    }
}
