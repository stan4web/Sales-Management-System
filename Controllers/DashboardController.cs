using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApp.Data;

namespace SalesApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Dashboard = "active";
            //get the current week
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int days = day - DayOfWeek.Monday;
            DateTime startweek = DateTime.Now.AddDays(-days);
            DateTime endweek = startweek.AddDays(6);

            //get the current month
            DateTime startmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endmonth = startmonth.AddMonths(1).AddDays(-1);

            //get 1 to 3 month 
            DateTime start3month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime end3month = start3month.AddMonths(3).AddDays(-1);


            //get daily account statement
            var dailyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date == DateTime.Now.Date)
                .ToListAsync();
            if (dailyaccountstatement.Count() > 0)
            {
                ViewBag.DailyAccount = dailyaccountstatement.Sum(x => x.TotalAmount);
            }
            else
            {
                ViewBag.DailyAccount = 0;
            }


            //get weekly statement of account made by cashier every day
            var weeklyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date >= startweek.Date && x.DateCreated.Date <= endweek.Date )
                .ToListAsync();

            if(weeklyaccountstatement.Count() > 0)
            {
                ViewBag.WeeklyAccount = weeklyaccountstatement.Sum(x => x.TotalAmount);
            }
            else
            {
                ViewBag.WeeklyAccount = 0;
            }

            //get monthly statement of account made by cashier every day
            var monthlyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date >= startmonth.Date && x.DateCreated.Date <= endmonth.Date)
                .ToListAsync();

            if (monthlyaccountstatement.Count() > 0)
            {
                ViewBag.MonthlyAccount = monthlyaccountstatement.Sum(x => x.TotalAmount);
            }
            else
            {
                ViewBag.MonthlyAccount = 0;
            }

            //get total worth of Goods in a stock
            var allproduct = await _context.Products.ToListAsync();
            if(allproduct.Count() > 0)
            {
                ViewBag.TotalWorth= allproduct.Sum(x => x.PurchasePrice * x.Quantity);
            }

            var threemonthexpiration = allproduct
                .Where(x => x.ExpirationDate.Value.Date >= start3month.Date && x.ExpirationDate.Value.Date <= end3month.Date);
            ViewBag.GetEpiringDate = threemonthexpiration.ToList();

            var expired = allproduct
                .Where(x => x.ExpirationDate.Value.Date < DateTime.Now.Date && x.Quantity > 0);
            ViewBag.GetEpiredProduct = expired.ToList();

            //get all receipt
            var getreceipt = await _context.Receipts
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToListAsync();
            ViewBag.Receipt = getreceipt;

            //get payments
            var payment = await _context.AccountStatements
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToListAsync();
            ViewBag.Payment = payment;

            //get product reorder level
            var reoderleve = allproduct
                .Where(x => x.Quantity <= x.ReOrderLevel)
                .ToList();
            ViewBag.ReorderLevel = reoderleve;

            //get cashiers
            var cashier = await _context.Users.ToListAsync();
            ViewBag.Cashier = cashier;

            //get order list
            var getorder = await _context.Orders
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            ViewBag.Order = getorder;


            return View();
        }

        public async Task<IActionResult> DailyTransactionDetails()
        {
            //get daily account statement
            var dailyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date == DateTime.Now.Date)
                .ToListAsync();
            if (dailyaccountstatement.Count() > 0)
            {
                ViewBag.DailyAccount = dailyaccountstatement.Sum(x => x.TotalAmount);
                ViewBag.DailyPropit = dailyaccountstatement.Sum(s => s.Profit);
            }
            else
            {
                ViewBag.DailyAccount = 0;
                ViewBag.DailyPropit = 0;
            }
            
            var dailysalest = await _context.Sales
                .Where(x => x.DateCreated.Date == DateTime.Now.Date)
                .OrderByDescending(s=>s.Id)
                .ToListAsync();
            return View(dailysalest);
        }

        public async Task<IActionResult> WeeklyTransactionDetails()
        {
            //get the current week
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int days = day - DayOfWeek.Monday;
            DateTime startweek = DateTime.Now.AddDays(-days);
            DateTime endweek = startweek.AddDays(6);

            ViewBag.Week =" From "+ startweek.Date.ToShortDateString() + " - " + endweek.Date.ToShortDateString();
            //get the current month
            DateTime startmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endmonth = startmonth.AddMonths(1).AddDays(-1);

            //get daily account statement
            var weeklyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date >= startweek.Date && x.DateCreated.Date <= endweek.Date)
                .ToListAsync();
            if (weeklyaccountstatement.Count() > 0)
            {
                ViewBag.WeeklyAccount = weeklyaccountstatement.Sum(x => x.TotalAmount);
                ViewBag.WeeklyPropit = weeklyaccountstatement.Sum(s => s.Profit);
            }
            else
            {
                ViewBag.WeeklyAccount = 0;
                ViewBag.WeeklyPropit = 0;
            }

            var weeklysalest = await _context.Sales
                .Where(x => x.DateCreated.Date >= startweek.Date && x.DateCreated.Date <= endweek.Date)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            return View(weeklysalest);
        }

        public async Task<IActionResult> MonthlyTransactionDetails()
        {
           
            //get the current month
            DateTime startmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endmonth = startmonth.AddMonths(1).AddDays(-1);

            ViewBag.Month = endmonth.ToShortDateString();

            //get daily account statement
            var monthlyaccountstatement = await _context.AccountStatements
                .Where(x => x.DateCreated.Date >= startmonth.Date && x.DateCreated.Date <= endmonth.Date)
                .ToListAsync();
            if (monthlyaccountstatement.Count() > 0)
            {
                ViewBag.MonthlyAccount = monthlyaccountstatement.Sum(x => x.TotalAmount);
                ViewBag.MonthlyPropit = monthlyaccountstatement.Sum(s => s.Profit);
            }
            else
            {
                ViewBag.MonthlyAccount = 0;
                ViewBag.MonthlyPropit = 0;
            }

            var monthlysalest = await _context.Sales
                .Where(x => x.DateCreated.Date >= startmonth.Date && x.DateCreated.Date <= endmonth.Date)
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            return View(monthlysalest);
        }

        public async Task<IActionResult> InvoiceView(string id)
        {
            ViewBag.ReceiptNo = id;
            var sales = await _context.Sales
                .Where(x => x.ReceiptNumber==id)
                .ToListAsync();
            return View(sales);
        }

        public async Task<IActionResult> PaymentView()
        {
            var statement = await _context.AccountStatements
                .OrderByDescending(x=>x.Id)
                .ToListAsync();
            return View(statement);
        }
    }
}