using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NewsManage.Models;
using OrchardCore.DisplayManagement.ModelBinding;

namespace NewsManage.ViewModels
{
   public class TypeNewClassifyPartVM
    {
        public string NewClassifyType { get; set; } 

        public TypeNewClassifyPart typeNewClassifyPart { get; set; }

        [BindNever]
        public IUpdateModel Updater { get; set; }
    }
}
