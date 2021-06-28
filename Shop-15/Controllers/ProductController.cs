using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_15.Data;
using Shop_15.Models;
using Shop_15.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private bool Valid;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _db.Product.Include(u => u.Category);
            return View(productList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };

            if (id == null)
            {
                Valid = true;
                return View(productVM);
            }
            else
            {
                Valid = false;
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            if (ModelState.IsValid)
            {
                if (Valid == false)
                {
                    _db.Product.Update(product);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {

                    _db.Product.Add(product);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(product);
        }

        // GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = _db.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var product = _db.Product.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                _db.Product.Remove(product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}