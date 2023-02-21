using System;
using System.Collections.Generic;

#nullable disable

namespace GrpcService.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal? Price { get; set; } = 0;
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public bool? IsDelete { get; set; } = false;

        public virtual Category Category { get; set; }
    }
}
