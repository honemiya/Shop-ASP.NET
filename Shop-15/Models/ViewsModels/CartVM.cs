using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Models.ViewsModels
{
    public class CartVM
    {
        public CartVM()
        {
            ProductList = new List<Product>();
        }
        
        public AppUser AppUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
