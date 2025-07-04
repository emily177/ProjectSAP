﻿namespace ProjectSAP.Models
{
    public class ItemModel
    {
        public string ItemCode { get;set; }
        public string? ItemName { get; set; }
        public double? Price { get; set; }
        public double Quantity { get; set; }
        public double? InStock { get; set; } 
        //public double? TotalPrice { get { return Price * Quantity; } }

        public ItemModel()
        {
            ItemCode = string.Empty;
            ItemName = string.Empty;
            Price = 0.0;
            Quantity = 0.0;
            InStock = 0.0;
        }
        public ItemModel(string itemCode, string itemName, double price, double quantity, double inStock)
        {
            ItemCode = itemCode;
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
            InStock = inStock;
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
