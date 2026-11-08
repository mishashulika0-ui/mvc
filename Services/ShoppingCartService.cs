using p48_lessons_asp.Models;

namespace p48_lessons_asp.Services
{
    public class ShoppingCartService
    {
        private readonly ShoppingCart _cart = new();

        public void AddItem(string itemName)
        {
            var existingItem = _cart.Items.FirstOrDefault(i => i.Name == itemName);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _cart.Items.Add(new CartItem { Name = itemName, Quantity = 1 });
            }
        }

        public ShoppingCart GetCart()
        {
            return _cart;
        }
    }
}