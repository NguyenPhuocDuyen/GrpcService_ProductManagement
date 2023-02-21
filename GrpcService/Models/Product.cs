using System;
using System.Collections.Generic;

#nullable disable

namespace GrpcService.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Category Category { get; set; }
    }
}
