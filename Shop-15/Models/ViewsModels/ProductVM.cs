using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Models.ViewsModels
{
    public class ProductVM
    {
        public Products Product { get; set; }
        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}
