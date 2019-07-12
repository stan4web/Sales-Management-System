using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalingPrice { get; set; }
        public decimal Profit { get; set; }
        public string SoldBy { get; set; }
        public string ReceiptNumber { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
