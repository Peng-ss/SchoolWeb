﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;

namespace OrchardCore.Deployment.Controllers
{
    [Admin]
    public class ImportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
