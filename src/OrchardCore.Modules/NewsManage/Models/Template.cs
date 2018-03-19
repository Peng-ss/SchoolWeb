using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace NewsManage.Models
{
    public class Template 
    {
        public string touser { get; set; }

        public string template_id { get; set; }
        
        public string url { get; set; }

        public TemplateData data { get; set; }
    }
}
