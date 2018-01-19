using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using OrchardCore.ContentManagement;

namespace NewsManage.Models
{
    public class TypeNewClassifyPart :ContentPart
    {
        public string NewClassifyType { get; set; }
    }
}
