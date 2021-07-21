using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_15.Data;
using Shop_15.Models;
using Shop_15.Models.ViewsModels;
using Shop_15.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvirinment;
        private readonly IEmailSender _emailSender;

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvirinment = webHostEnvironment;
            _emailSender = emailSender;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Order));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Order")]
        public async Task<IActionResult> OrderPost(CartVM productUserVM)
        {
            var PathToTemplate = _webHostEnvirinment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString()
                + "OrderConfirmation.html";

            var subject = "New order";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productsSB = new StringBuilder();

            foreach (var item in productUserVM.ProductList)
            {
                productsSB.Append($"Name {item.Name}\tPrice {item.Price}");
            }

            string messageBody = string.Format(
               HtmlBody,
               productUserVM.AppUser.UserName,
               productUserVM.AppUser.Email,
               productsSB.ToString()
               );


            await _emailSender.SendEmailAsync(ENW.AdminEmail, subject, messageBody);

            return RedirectToAction(nameof(OrderConfirmation));
        }

        public IActionResult OrderConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }
    }
}
