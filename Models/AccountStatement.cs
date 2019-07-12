using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class AccountStatement
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string CashierName { get; set; }
        public string CustomerName { get; set; }
        public decimal Payable { get; set; }
        public decimal Discount { get; set; }
        public decimal AdvancePaid { get; set; }
        public decimal Balance { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal Profit { get; set; }
        public DateTime DateCreated { get; set; }

    }
}
