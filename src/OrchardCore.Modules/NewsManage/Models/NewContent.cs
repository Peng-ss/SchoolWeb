using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace NewsManage.Models
{
    public class NewContent :ContentPart
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public new string Content { get; set; }
    }
}
