using FoodProject.Enums;
using FoodProject.Models;

namespace FoodProject.ViewModel
{
    public class StatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }

        public Dictionary<OrderStatus, int> OrdersByStatus { get; set; } = new();

        public List<Order> RecentOrders { get; set; } = new();

        public List<TopProductViewModel> TopProducts { get; set; } = new();

        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; } = new();
    }

    public class TopProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyRevenueViewModel
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

}
