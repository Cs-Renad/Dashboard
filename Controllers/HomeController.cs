using System.Diagnostics;
using Dashboard.Data;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateProducts(Products productss)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productss);
                _context.SaveChanges();
                TempData["Add"] = " „  «·≈÷«›… »‰Ã«Õ";
                return RedirectToAction("Addnewitems");

            }
            TempData["Add"] = "·„   „ «·≈÷«›… Ì—ÃÏ «· √ﬂœ „‰ ’Õ… «·„œŒ·« ";


            var productsss = _context.products.ToList();
            return View("Addnewitems", productsss);
           
        }


        public IActionResult AddDemag(Damegedproducts damege)
        {

            _context.Add(damege);
            _context.SaveChanges();

            return RedirectToAction("Demag");
        }
        public IActionResult Demag()
        {
            //ViewBag.Username = HttpContext.Session.GetString("userdata");
            var products = _context.products.ToList();

            var Productsdemage = _context.damegedproducts.Join
                (
                     _context.products,

                      demag => demag.ProductId,
                      products => products.Id,


                     (demag, products) => new
                     {
                         demag,
                         products
                     }

                ).Join
                (
                  _context.productsDetails,

                  p => p.demag.ProductId,
                  c => c.ProductId,

                  (p, c) => new
                  {
                      id = p.demag.Id,
                      name = p.products.Name,
                      color = c.Color,
                      qty = p.demag.Qty
                  }
                  ).ToList();

            Console.WriteLine($"{Productsdemage}");


            ViewBag.products = products;
            ViewBag.damage = Productsdemage;


            return View();
        }


        public IActionResult Addnewitems()
        {
            var products = _context.products.ToList();  //Read
            ViewBag.products = products;
            return View(products);
            
        }


        public IActionResult Delete(int record_no)
        {
            var productdel = _context.products.SingleOrDefault(p => p.Id == record_no);//serch on record 

            if (productdel != null)
            {
                _context.products.Remove(productdel);
                _context.SaveChanges();
                TempData["del"] = true;
            }

            else
            {
                TempData["del"] = false;
            }


            return RedirectToAction("Addnewitems");

        }

        public IActionResult Update(Products product)
        {
            if (ModelState.IsValid)
            {
                _context.products.Update(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Addnewitems");
        }
        
        public IActionResult Edit(int record_no)
        {
            var Product = _context.products.SingleOrDefault(x => x.Id == record_no);

            return View(Product);

        }
        
       public IActionResult CreateDeatils(ProductsDetails productsDetails, IFormFile photo)
        {
            //  Õﬁﬁ „‰ √‰ Ã„Ì⁄ «·ÕﬁÊ· «·„ÿ·Ê»… „ÊÃÊœ…
            if (photo == null || photo.Length == 0 ||
                productsDetails.Price <= 0 || productsDetails.Qty <= 0 || string.IsNullOrEmpty(productsDetails.Color))
            {
                TempData["ErrorMessage"] = "Ì—ÃÏ „·¡ ﬂ«„· «·ÕﬁÊ· «·„ÿ·Ê»… ··„‰ Ã";
                return RedirectToAction("ProductsDetails");
            }

            bool productExists = _context.productsDetails.Any(p => p.ProductId == productsDetails.ProductId);

            if (productExists)
            {
                TempData["ErrorMessage"] = "·ﬁœ  „ «÷«›…  ›«’Ì· ··„‰ Ã „”»ﬁ« !";
                return RedirectToAction("ProductsDetails");
            }

            var fileName = photo.FileName;
            var dir = "img";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, dir, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                photo.CopyTo(stream);
                stream.Close();
            }

            productsDetails.Images = dir + "/" + fileName;

            _context.productsDetails.Add(productsDetails);
            _context.SaveChanges();

            TempData["SuccessMessage"] = " „ ≈÷«›…  ›«’Ì· «·„‰ Ã »‰Ã«Õ!";
            return RedirectToAction("ProductsDetails");

        }

      

        
        public JsonResult EditProductsDetails(int id)
        {
            var product = _context.productsDetails
                  .FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                return Json(product);
            }
            else
            {
                return Json(null);
            }


        }
        
        [Route("update-products-details")]
        public IActionResult UpdateProductsDetails(ProductsDetails productsDetail, IFormFile photo)
        {
            // «·⁄ÀÊ— ⁄·Ï «·„‰ Ã «·„ÊÃÊœ »«” Œœ«„ Product_Id
            var existingProduct = _context.productsDetails.SingleOrDefault(p => p.ProductId == productsDetail.ProductId);

            if (existingProduct == null)
            {
                return RedirectToAction("ProductsDetails");
                // „⁄«·Ã… Õ«·… ⁄œ„ «·⁄ÀÊ—
            }

            if (photo != null && photo.Length > 0)
            {
                var fileName = photo.FileName;
                var dir = "img";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, dir, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                productsDetail.Images = dir + "/" + fileName;
                existingProduct.Images = productsDetail.Images; // «” Œœ«„ «·’Ê—… «·„ÕœÀ… ›ﬁÿ ≈–«  „ ≈œŒ«·Â«
            }
            // «· Õﬁﬁ «·ÌœÊÌ „‰ «·ÕﬁÊ· «·„ÿ·Ê»…
            if (productsDetail.Price > 0 && productsDetail.Qty > 0 && !string.IsNullOrEmpty(productsDetail.Color))
            {
                //  ÕœÌÀ  ›«’Ì· «·„‰ Ã
                existingProduct.Price = productsDetail.Price;
                existingProduct.Qty = productsDetail.Qty;
                existingProduct.Color = productsDetail.Color;

                //  ÕœÌÀ «·„‰ Ã ›Ì ﬁ«⁄œ… «·»Ì«‰« 
                _context.productsDetails.Update(existingProduct);
                _context.SaveChanges();

                // ÷»ÿ —”«·… «·‰Ã«Õ
                TempData["SuccessMessage"] = " „  ÕœÌÀ «·„‰ Ã »‰Ã«Õ !";
            }
            else
            {
                // ÷»ÿ —”«·… «·Œÿ√
                TempData["ErrorMessage"] = "Ì—ÃÏ «· √ﬂœ „‰ «·„⁄·Ê„«  «·„œŒ·… !";

            }


            return RedirectToAction("ProductsDetails");

        }
       
        [Route("delete-product-details")]
        public IActionResult DeleteProductsDetails(int product_id)
        {
            // «·⁄ÀÊ— ⁄·Ï «·„‰ Ã «·„—«œ Õ–›Â „‰ ÃœÊ· ProductsDetails
            var productDel = _context.productsDetails.SingleOrDefault(p => p.ProductId == product_id);

            if (productDel != null)
            {
                // Õ–› «·„‰ Ã „‰ ÃœÊ· ProductsDetails
                _context.productsDetails.Remove(productDel);
                _context.SaveChanges();
                TempData["SuccessMessage"] = " „ Õ–› «·„‰ Ã »‰Ã«Õ.";
            }
            else
            {
                TempData["ErrorMessage"] = "·„ Ì „ «·⁄ÀÊ— ⁄·Ï «·„‰ Ã";
            }

            return RedirectToAction("ProductsDetails");

        }
        public IActionResult ProductsDetails()
        {
            /* var productsDetails = _context.ProductsDetails.ToList(); //Get All Products 
             var products = _context.Products.ToList(); //Get All Products */

            var productsDetails = _context.productsDetails.Join(
                    _context.products,
                    products_details => products_details.ProductId,
                    products => products.Id,

                    (products_details, products) => new
                    {
                        Product_Id = products_details.ProductId,
                        Name = products.Name,
                        Color = products_details.Color,
                        Price = products_details.Price,
                        Qty = products_details.Qty,
                        Images = products_details.Images
                    }

                ).ToList();

            var products = _context.products.ToList();
            ViewBag.products = products;
            ViewBag.productsDetails = productsDetails;
            return View("ProductsDetails");
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
