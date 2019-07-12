using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal? TotalPrice { get; set; }
        [Display(Name = "Selling Price")]
        public decimal? SalingPrice { get; set; }

        public int ReOrderLevel { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string ImageUrl { get; set; }
        public bool ProductStatus { get; set; }
        public int? SubCategoryId { get; set; }
        public string Description { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public string Status { get; set; }

        public virtual List<Order> Orders { get; set; }
    }
}
