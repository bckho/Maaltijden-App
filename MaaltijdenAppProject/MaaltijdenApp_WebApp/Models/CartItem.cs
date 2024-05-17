using MaaltijdenApp_Core.Models;

namespace MaaltijdenApp_WebApp.Models
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }

        public CartItem()
        {
        }

        public CartItem(Product product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
        }
    }
}
