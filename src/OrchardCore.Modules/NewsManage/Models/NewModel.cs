using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace NewsManage.Models
{
    public class NewModel : ContentPart
    {
        public string TitlePart { get; set; }
        public string BodyPart { get; set; }
        public string Author { get; set; }

        public Boolean Published { get; set; }
    }
}
