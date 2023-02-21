using Grpc.Core;
using GrpcService.Models;
using MyProto;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MyProto.GrpcProduct;

namespace GrpcService
{
    public class ProductService : GrpcProductBase
    {
        private readonly GrpcServiceContext _db;

        public ProductService(GrpcServiceContext db)
        {
            _db = db;
        }

        public override Task<ProductList> GetAll(Empty request, ServerCallContext context)
        {
            var response = new ProductList();

            var producrs = from obj in _db.Products
                               //where obj.IsDelete == false
                           select new MyProto.Product
                           {
                               Id = obj.Id,
                               CategoryId = obj.CategoryId,
                               Name = obj.Name,
                               Price = (double)obj.Price,
                               CreateAt = DateTimeToTimestamp(obj.CreateAt),
                               UpdateAt = DateTimeToTimestamp(obj.UpdateAt),
                               IsDelete = obj.IsDelete ?? false
                           };

            response.Products.AddRange(producrs);

            return Task.FromResult(response);
        }

        public override Task<MyProto.Product> GetProduct(IdRequest request, ServerCallContext context)
        {
            var product = _db.Products.Find( int.Parse(request.Id));

            if (product == null || product.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Product not found");
                throw new RpcException(status);
            }

            MyProto.Product response = new()
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Price = (double)product.Price,
                CreateAt = DateTimeToTimestamp(product.CreateAt),
                UpdateAt = DateTimeToTimestamp(product.UpdateAt),
                IsDelete = product.IsDelete ?? false
            };

            return Task.FromResult(response);
        }

        public override Task<Empty> Create(MyProto.Product request, ServerCallContext context)
        {
            Models.Product product = new()
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Price = (decimal)request.Price,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDelete = false
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Update(MyProto.Product request, ServerCallContext context)
        {
            var product = _db.Products.Find(request.Id);

            if (product == null || product.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Product not found");
                throw new RpcException(status);
            }

            product.CategoryId = request.CategoryId;
            product.Name = request.Name;
            product.Price = (decimal)request.Price;
            product.UpdateAt = DateTime.Now;

            _db.Products.Update(product);
            _db.SaveChanges();

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Delete(IdRequest request, ServerCallContext context)
        {
            var product = _db.Products.Find(int.Parse(request.Id));
            if (product == null || product.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Product not found");
                throw new RpcException(status);
            }
            product.IsDelete = true;
            _db.Products.Update(product);
            _db.SaveChanges();

            return Task.FromResult(new Empty());
        }

        //private static DateTime? TimestampToDateTime(Google.Protobuf.WellKnownTypes.Timestamp timestamp)
        //{
        //    if (timestamp == null)
        //    {
        //        return null;
        //    }

        //    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp.Seconds);
        //    dateTimeOffset = dateTimeOffset.AddTicks(timestamp.Nanos / 100);
        //    return dateTimeOffset.UtcDateTime;
        //}

        private static Google.Protobuf.WellKnownTypes.Timestamp DateTimeToTimestamp(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }

            var dateTimeOffset = new DateTimeOffset(dateTime.Value.ToUniversalTime());
            var seconds = dateTimeOffset.ToUnixTimeSeconds();
            var nanos = dateTimeOffset.Ticks % TimeSpan.TicksPerSecond * 100;

            return new Google.Protobuf.WellKnownTypes.Timestamp
            {
                Seconds = seconds,
                Nanos = (int)nanos
            };
        }


    }
}
