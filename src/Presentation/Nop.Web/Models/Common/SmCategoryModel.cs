﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Web.Models.Common
{
    public class SmCategoryModel
    {
        public string Title { get; set; }

        public string SeName { get; set; }

        public IList<SmCategoryModel> SubCategories { get; set; }
    }
}