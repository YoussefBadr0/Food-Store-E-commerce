using FoodProject.Data;
using FoodProject.Enums;
using FoodProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;

            var monthlyRevenueData = await _context.Orders
                .Where(x => x.OrderDate >= now.AddMonths(-6) && x.Status != OrderStatus.Cancelled)
                .GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(x => x.TotalAmount)
                })
                .ToListAsync();

            var model = new StatisticsViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalRevenue = await _context.Orders
                    .Where(x => x.Status != OrderStatus.Cancelled)
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
                OrdersByStatus = await _context.Orders
                    .GroupBy(x => x.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count),
                RecentOrders = await _context.Orders
                    .Include(x => x.AppUser)
                    .OrderByDescending(x => x.OrderDate)
                    .Take(8)
                    .ToListAsync(),
                TopProducts = await _context.OrderItems
                    .GroupBy(x => x.ProductNameSnapshot)
                    .Select(g => new TopProductViewModel
                    {
                        ProductName = g.Key,
                        TotalSold = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.Quantity * x.UnitPriceSnapshot)
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(5)
                    .ToListAsync(),
                MonthlyRevenue = monthlyRevenueData
                    .Select(x => new MonthlyRevenueViewModel
                    {
                        Month = $"{x.Year}-{x.Month:D2}",
                        Revenue = x.Revenue
                    })
                    .OrderBy(x => x.Month)
                    .ToList()
            };

            return View(model);
        }
    }
}