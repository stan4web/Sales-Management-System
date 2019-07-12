using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesApp.Data;
using SalesApp.Models;

namespace SalesApp.Controllers
{
    [Authorize]
    public class POSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public POSController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var category = await _context.Categories.ToListAsync();
           
            return View(category);
        }

       
        public async Task<IActionResult> Subcategory(int id=0)
        {
            var subcategories = await _context.SubCategories.Where(s=>s.CategoryId==id).ToListAsync();

            return View(subcategories);
        }
       
        public async Task<IActionResult> ProductListAll()
        {
            var products =await _context.Products
                .OrderBy(s=>s.Name)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductList(string receiptnumber, int subCatId = 0, int catId = 0)
        {
            var userDetails = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var order = await _context.Orders.Where(x => x.OrderReferenceId == userDetails.Id).ToListAsync();
            ViewBag.ReceiptSession = receiptnumber;
            ViewBag.SubCategoryId = subCatId;
            ViewBag.CategoryId = catId;
            return View(order);
        }

        public async Task<IActionResult> ProductDetails(int id=0, int subCatId=0, int catId=0)
        {
            
            ViewBag.SubCategoryId = subCatId;
            ViewBag.CategoryId = catId;

            var products = await _context.Products.SingleOrDefaultAsync(s=>s.Id==id);
            if(products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> OrderList(string receiptnumber, string page, int subCatId = 0, int catId = 0, int productId=0,  decimal amount=0)
        {
            int maxSize = 10;
            ViewBag.ReceiptSession=  receiptnumber;
            char[] chars = new char[62];
            string a;
            a = "123456789abcdefghijklmnopqrstuvwxyz";

            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder coderesult = new StringBuilder(size);
            foreach (byte b in data)
            {
                coderesult.Append(chars[b % (chars.Length - 1)]);
            }


           

            // Requires: using Microsoft.AspNetCore.Http;
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("ReceiptNumber")))
            {
                HttpContext.Session.SetString("ReceiptNumber", coderesult.ToString().ToUpper());
                
            }

            string getsession = HttpContext.Session.GetString("ReceiptNumber");

            var product = await _context.Products.FirstOrDefaultAsync(c => c.Id == productId);

            var userDetails = await _context.Users.FirstOrDefaultAsync(z => z.UserName == User.Identity.Name);
            ViewBag.ReceiptSession = getsession;
            //check if product exist
            var checkexist = await _context.Orders.FirstOrDefaultAsync(x => x.ProductId == product.Id && x.IsPaid==false && x.OrderReferenceId==userDetails.Id);
            if(checkexist != null)
            {
                checkexist.Quantity = checkexist.Quantity + 1;
            }
            else
            {
                var myorder = new Order
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    Amount = product.SalingPrice.Value,
                    OrderReferenceId = userDetails.Id,
                    DateCreated = DateTime.Now,
                    ReceiptNumber = getsession,
                };
                await _context.AddAsync(myorder);
            }
           
           
            await _context.SaveChangesAsync();
            if (page == "All")
            {
                return RedirectToAction("ProductListAll");
            }
            else
            {
                return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
            }
            
        }

        
        [HttpPost]
        public async Task<IActionResult> OrderDelete(string receiptnumber,int id=0,int subCatId = 0, int catId = 0, int productId = 0, decimal amount = 0)
        {
            ViewBag.ReceiptSession = receiptnumber;
             var order = await _context.Orders.SingleOrDefaultAsync(c => c.Id == id);
            var userDetails = await _context.Users.FirstOrDefaultAsync(a => a.UserName == User.Identity.Name);
           
            
            _context.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
        }

        [HttpPost]
        public async Task<IActionResult> ClearOrderList(string receiptnumber, int id = 0, int subCatId = 0, int catId = 0, int productId = 0, decimal amount = 0)
        {
            ViewBag.ReceiptSession= receiptnumber;
            string ReceiptNumber = String.Empty;
            HttpContext.Session.Clear();

            var userDetails = await _context.Users.FirstOrDefaultAsync(a => a.UserName == User.Identity.Name);
            var order =  _context.Orders.Where(c => c.OrderReferenceId==userDetails.Id && c.IsPaid==false);
            foreach(var item in order)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
        }

        [HttpPost]
        public async Task<IActionResult> AddQty(string receiptnumber, int id = 0, int subCatId = 0, int catId = 0, int productId = 0, decimal amount = 0)
        {
            ViewBag.ReceiptSession=  receiptnumber;
             var getorder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            getorder.Quantity = getorder.Quantity + 1;
            getorder.Amount = getorder.Amount + amount;
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveQty(string receiptnumber,int id = 0, int subCatId = 0, int catId = 0, int productId = 0, decimal amount = 0)
        {
            ViewBag.ReceiptSession=  receiptnumber;
             var getorder = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if(getorder.Quantity > 1)
            {
                getorder.Quantity = getorder.Quantity - 1;
                getorder.Amount = getorder.Amount - amount;
                await _context.SaveChangesAsync();
            }
           
            return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string customername, string receiptnumber, decimal discount, decimal payable, int id = 0, int subCatId = 0, int catId = 0, int productId = 0, decimal amount = 0)
        {
            var userDetail = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            var orderlist = await _context.Orders.Where(x => x.IsPaid == false && x.OrderReferenceId == userDetail.Id)
                .Include(z=>z.Product)
                .ToListAsync();

            var getsale = await _context.Sales.FirstOrDefaultAsync(x => x.ReceiptNumber.ToUpper() == receiptnumber.ToUpper());

            if (getsale != null)
            {
                TempData["paymentError"] = "Sorry duplication of sales if not allowed. please clear invoice list.";
                return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
            }
            foreach(var orderItem in orderlist)
            {
                decimal myprofit = orderItem.Product.SalingPrice.Value - orderItem.Product.PurchasePrice;
                decimal sumprofit = myprofit * orderItem.Quantity;

                //update product in stock
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderItem.ProductId);
                if (product.Quantity < orderItem.Quantity)
                {
                    TempData["paymentError"] = $"This quantity you selected is more than the quantity in stuck ({orderItem.Product.Name}) ";
                    return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
                }

                product.Quantity = product.Quantity - orderItem.Quantity;

                
                string getsession = HttpContext.Session.GetString("ReceiptNumber");
                var saleresult = new List<Sale>{
                     new Sale
                       {
                         ProductName=orderItem.Product.Name,
                         Quantity=orderItem.Quantity,
                         PurchasePrice=orderItem.Product.PurchasePrice,
                         SalingPrice=orderItem.Product.SalingPrice.Value,
                         Profit=sumprofit,
                         SoldBy =userDetail.Id,
                         ReceiptNumber=receiptnumber,
                         DateCreated =DateTime.Now,
                         
                       }
                };

                await _context.Sales.AddRangeAsync(saleresult);
            }

            await _context.SaveChangesAsync();
            var getprofilefromsales = await _context.Sales.Where(x => x.ReceiptNumber == receiptnumber).ToArrayAsync();
            //get total profit
            if (getprofilefromsales.Count() > 0)
            {
                ViewBag.Profit = getprofilefromsales.Sum(x => x.Profit) -discount;
            }
            else
            {
                ViewBag.Profit = 0;
            }
            var salesreceipt = new Receipt
            {
                CashierName = userDetail.UserName,
                Discount = discount,
                BalanceAmount = payable,
                AmountReceived = amount,
                CustomerName = customername,
                ReceiptNumber = receiptnumber,
                DateCreated = DateTime.Now,
                TotalAmount = amount,
                Profit = Convert.ToDecimal(ViewBag.Profit),
            };

            var accountstatement = new AccountStatement
            {
                AdvancePaid = amount,
                TotalAmount = amount,
                Balance = 0,
                CashierName = userDetail.Id,
                CustomerName = customername,
                Discount = discount,
                Payable = payable,
                ReceiptNumber = receiptnumber,
                DateCreated = DateTime.Now,
                Profit=Convert.ToDecimal(ViewBag.Profit),

            };

            await _context.AccountStatements.AddAsync(accountstatement);
            await _context.Receipts.AddAsync(salesreceipt);
            await _context.SaveChangesAsync();
            TempData["payment"] = "Successful";
            return RedirectToAction("ProductList", new { subCatId, catId, receiptnumber });
        }

        public async Task<IActionResult> Print(string receiptnumber, int subCatId = 0, int catId = 0)
        {
            var userDetails = await _context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var order = await _context.Orders.Where(x => x.OrderReferenceId == userDetails.Id).ToListAsync();
            ViewBag.ReceiptSession = receiptnumber;
            ViewBag.SubCategoryId = subCatId;
            ViewBag.CategoryId = catId;
            return View(order);
        }


    }
}