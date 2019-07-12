using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string OrderReferenceId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsPaid { get; set; }
        public string  ReceiptNumber { get; set; }

    }
}
