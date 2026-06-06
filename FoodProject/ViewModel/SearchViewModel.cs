using FoodProject.Models;

namespace FoodProject.ViewModel
{
    public class SearchViewModel
    {
        public string? Query { get; set; }

        public IEnumerable<Product> Products { get; set; }
            = Enumerable.Empty<Product>();

        
    }
}
