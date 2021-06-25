using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_15.Data;
using Shop_15.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET
        public IActionResult Index()
        {
            IEnumerable<Products> productsList = _db.Products.Include(x => x.Category);
            return View(productsList);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(products);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(products);
        }
    }
}
