using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using MyProto;
using static MyProto.GrpcCategory;

namespace GrpcClient.Controllers
{
    public class CategoryController : Controller
    {
        static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        GrpcCategoryClient categoryClient = new GrpcCategoryClient(channel);

        public IActionResult Index()
        {
            try
            {
                //get category list
                var data = categoryClient.GetAll(new MyProto.Empty());
                return View(data);
            }
            catch { }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(string id)
        {
            try
            {
                //get category
                var data = categoryClient.GetCategory(new IdRequest { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            try
            {
                //add category
                categoryClient.Create(category);
                return RedirectToAction(nameof(Index));
            }
            catch { }

            return View();
        }

        public IActionResult Edit(string id)
        {
            try
            {
                //get category to edit
                var data = categoryClient.GetCategory(new IdRequest { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            try
            {
                //update category
                categoryClient.Update(category);
                return RedirectToAction(nameof(Index));
            }
            catch { }

            return View();
        }

        public IActionResult Delete(string id)
        {
            try
            {
                //get category to delete
                var data = categoryClient.GetCategory(new IdRequest() { Id = id });
                return View(data);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id, Category category)
        {
            try
            {
                //delete category
                categoryClient.Delete(new IdRequest { Id = id });
                return RedirectToAction("Index");
            } catch { }

            return View();
        }
    }
}
