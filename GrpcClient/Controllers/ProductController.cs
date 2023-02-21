using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyProto;
using static MyProto.GrpcCategory;
using static MyProto.GrpcProduct;

namespace GrpcClient.Controllers
{
    public class ProductController : Controller
    {
        static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        GrpcProductClient productCliet = new(channel);
        GrpcCategoryClient categoryClient = new(channel);

        public IActionResult Index()
        {
            try
            {
                //get list product
                var data = productCliet.GetAll(new Empty());
                return View(data);
            }
            catch { }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(string id)
        {
            try
            {
                //get product
                var data = productCliet.GetProduct(new IdRequest { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            try
            {
                //get category list
                var data = categoryClient.GetAll(new Empty());
                ViewData["CategoryId"] = new SelectList(data.Categorys, "Id", "Name");
                return View();
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            try
            {   
                //add product
                productCliet.Create(product);
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction(nameof(Create));
        }

        public IActionResult Edit(string id)
        {
            try
            {
                //get category list
                CategoryList categoryData = categoryClient.GetAll(new Empty());
                ViewData["CategoryId"] = new SelectList(categoryData.Categorys, "Id", "Name");

                var data = productCliet.GetProduct(new IdRequest { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            try
            {
                //update product
                productCliet.Update(product);
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction(nameof(Edit));
        }

        public IActionResult Delete(string id)
        {
            try
            {
                //get product to delete
                var data = productCliet.GetProduct(new IdRequest() { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id, IFormCollection colletion)
        {
            try
            {
                //delete product
                productCliet.Delete(new IdRequest { Id = id });
                return RedirectToAction("Index");
            }
            catch { }

            return RedirectToAction(nameof(Delete));
        }
    }
}
