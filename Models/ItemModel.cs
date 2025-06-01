namespace ProjectSAP.Models
{
    public class ItemModel
    {
        public string ItemCode { get;set; }
        public string? ItemName { get; set; }
        public double? Price { get; set; }
        public double Quantity { get; set; }

        public ItemModel()
        {
            ItemCode = string.Empty;
            ItemName = string.Empty;
            Price = 0.0;
            Quantity = 0.0;
        }
        public ItemModel(string itemCode, string itemName, double price, double quantity)
        {
            ItemCode = itemCode;
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
        }

        public ItemModel(string itemCode, double quantity)
        {
            ItemCode = itemCode;
            ItemName = "";
            Price = 0.0;
            Quantity = quantity; 
        }

    }
}
