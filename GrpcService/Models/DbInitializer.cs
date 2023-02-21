using System.Collections.Generic;
using System.Linq;

namespace GrpcService.Models
{
    public class DbInitializer
    {
        private readonly GrpcServiceContext _db;

        public DbInitializer(GrpcServiceContext db)
        {
            _db = db;
        }

        public void Initialize()
        {
            if (_db.Categories.Any())
            {
                return;
            }

            List<Category> categories = new()
            {
                new Category {Id = "c-1", Name = "Table"},
                new Category {Id = "c-2", Name = "Smart Phone"},
                new Category {Id = "c-3", Name = "Tivi"}
            };
            _db.Categories.AddRange(categories);
            _db.SaveChanges();

            List<Product> products = new()
            {
                new Product {CategoryId = "c-1", Name = "Big Table", Price = 999 },
                new Product {CategoryId = "c-1", Name = "Small Table", Price = 123 },
                new Product {CategoryId = "c-2", Name = "Iphone 25", Price = 2000 },
                new Product {CategoryId = "c-2", Name = "Sam Sung X", Price = 1599 },
                new Product {CategoryId = "c-3", Name = "SmartXEo", Price = 3212 }
            };
            _db.Products.AddRange(products);
            _db.SaveChanges();
        }
    }
}
