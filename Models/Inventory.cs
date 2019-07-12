using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityPurchase { get; set; }
        public int ReOrderLevel { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}
