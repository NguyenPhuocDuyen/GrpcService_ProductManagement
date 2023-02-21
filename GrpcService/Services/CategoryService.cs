using Grpc.Core;
using GrpcService.Models;
using MyProto;
using System.Threading.Tasks;
using System;
using System.Linq;
using static MyProto.GrpcCategory;

namespace GrpcService.Services
{
    public class CategoryService : GrpcCategoryBase
    {
        private readonly GrpcServiceContext _db;

        public CategoryService(GrpcServiceContext db)
        {
            _db = db;
        }

        public override Task<CategoryList> GetAll(Empty request, ServerCallContext context)
        {
            var response = new CategoryList();

            var categories = from obj in _db.Categories
                                 //where obj.IsDelete == false
                             select new MyProto.Category
                             {
                                 Id = obj.Id,
                                 Name = obj.Name,
                                 CreateAt = DateTimeToTimestamp(obj.CreateAt),
                                 UpdateAt = DateTimeToTimestamp(obj.UpdateAt),
                                 IsDelete = obj.IsDelete ?? false
                             };

            response.Categorys.AddRange(categories);

            return Task.FromResult(response);
        }

        public override Task<MyProto.Category> GetCategory(IdRequest request, ServerCallContext context)
        {
            var category = _db.Categories.Find(request.Id);

            if (category == null || category.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Category not found");
                throw new RpcException(status);
            }

            MyProto.Category response = new()
            {
                Id = category.Id,
                Name = category.Name,
                CreateAt = DateTimeToTimestamp(category.CreateAt),
                UpdateAt = DateTimeToTimestamp(category.UpdateAt),
                IsDelete = category.IsDelete ?? false
            };

            return Task.FromResult(response);
        }

        public override Task<Empty> Create(MyProto.Category request, ServerCallContext context)
        {
            Models.Category category = new()
            {
                Id = request.Id,
                Name = request.Name,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                IsDelete = false
            };

            _db.Categories.Add(category);
            _db.SaveChanges();

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Update(MyProto.Category request, ServerCallContext context)
        {
            var category = _db.Categories.Find(request.Id);

            if (category == null || category.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Category not found");
                throw new RpcException(status);
            }

            category.Name = request.Name;
            category.UpdateAt = DateTime.Now;

            _db.Categories.Update(category);
            _db.SaveChanges();

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Delete(IdRequest idRequest, ServerCallContext context)
        {
            var category = _db.Categories.Find(idRequest.Id);
            if (category == null || category.IsDelete == true)
            {
                var status = new Status(StatusCode.NotFound, "Category not found");
                throw new RpcException(status);
            }
            category.IsDelete = true;
            _db.Categories.Update(category);
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
