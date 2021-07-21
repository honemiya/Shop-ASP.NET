﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop_15.Data;
using Shop_15.Models;
using Shop_15.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop_15.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
                //Valid = true;
                return View(productVM);
            }
            else
            {
                //Valid = false;
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
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + ENW.ImagePath;
                string fileName = Guid.NewGuid().ToString();
                string extentions = Path.GetExtension(files[0].FileName);
                if (productVM.Product.Id == 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extentions), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.Image = fileName + extentions;
                    _db.Product.Add(productVM.Product);
                }
                else
                {
                    var formObject = _db.Product.AsNoTracking().FirstOrDefault(x => x.Id == productVM.Product.Id);
                    if(files.Count > 0)
                    {
                        var oldFile = Path.Combine(upload, formObject.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extentions), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extentions;
                    }
                    else
                    {
                        productVM.Product.Image = formObject.Image;
                    }
                    _db.Product.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _db.Category.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }); ; ;
            return View(productVM);
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
                string webRootPath = _webHostEnvironment.WebRootPath;
                string upload = webRootPath + ENW.ImagePath;
                string temp = upload + product.Image;
                if (System.IO.File.Exists(temp))
                {
                    System.IO.File.Delete(temp);
                }
                _db.Product.Remove(product);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}