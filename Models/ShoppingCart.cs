namespace p48_lessons_asp.Models
{
    public class CartItem
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();

    
        public int TotalItems => Items.Sum(i => i.Quantity);
    }
}