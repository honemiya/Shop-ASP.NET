using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shop_15.Data;
using Shop_15.Models;
using Shop_15.Models.ViewsModels;
using Shop_15.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // GET
        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Product.Include(x => x.Category),
                Categories = _db.Category
            };
            return View(homeVM);
        }

        // GET
        public IActionResult Details(int? id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }
            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(x => x.Category).Where(x => x.Id == id).FirstOrDefault(),
                InCart = false
            };
            foreach (var item in shoppingCartsList)
            {
                if(item.ProductId == id)
                {
                    detailsVM.InCart = true;
                }
            }
            return View(detailsVM);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }

            shoppingCartsList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(ENW.SessionCart, shoppingCartsList);
            return RedirectToAction("Index");
        }

        // GET
        public IActionResult RemoveFromCart(int? id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }

            var removeItem = shoppingCartsList.SingleOrDefault(x => x.ProductId == id);
            if (removeItem != null)
            {
                shoppingCartsList.Remove(removeItem);
            }

            HttpContext.Session.Set(ENW.SessionCart, shoppingCartsList);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
