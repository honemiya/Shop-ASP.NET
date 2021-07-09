using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_15.Data;
using Shop_15.Models;
using Shop_15.Models.ViewsModels;
using Shop_15.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }
            List<Product> list = new List<Product>();
            foreach (var item in shoppingCartsList)
            {
                Product product = _db.Product.Include(u => u.Category).FirstOrDefault(i => i.Id == item.ProductId);
                list.Add(product);
            }
            return View(list);

        }

        // GET
        public IActionResult Details(int? id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }
            DetailsVM detailsVM = new DetailsVM()
            {
                Product = _db.Product.Include(x => x.Category).Where(x => x.Id == id).FirstOrDefault()
            };
            return View(detailsVM);
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

        // GET
        public IActionResult Order()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var clime = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(ENW.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(ENW.SessionCart);
            }

            List<int> productInCart = shoppingCartsList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> productList = _db.Product.Where(x => productInCart.Contains(x.Id));

            CartVM cartVM = new CartVM()
            {
                AppUser = _db.AppUser.FirstOrDefault(i => i.Id == clime.Value),
                ProductList = productList.ToList()
            };

            return View(cartVM);
        }
    }
}
