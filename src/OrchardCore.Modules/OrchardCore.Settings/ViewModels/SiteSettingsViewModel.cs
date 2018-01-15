﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Settings.ViewModels
{
    public class SiteSettingsViewModel
    {
        public string SiteName { get; set; }
        public string TimeZone { get; set; }
        public IEnumerable<TimeZoneInfo> TimeZones { get; set; }
    }
}
