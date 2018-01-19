using System;
using System.Collections.Generic;
using System.Text;

namespace NewsManage.ViewModels
{
   public  class NewPartVM
    {
        public string NewID { get; set; }
        public string Name { get; set; }
        public string NewDisplayName { get; set; }
        public string NewDescription { get; set; }
        public Boolean Classify { get; set; }
    }
}
