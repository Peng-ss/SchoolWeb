using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ContentManagement;

namespace NewsManage.Models
{
    public class NewPart :ContentPart
    {
        public string Name { get; set; }

        public string NewDisplayName { get; set; }
        public string NewDescription { get; set; }

    }
}
