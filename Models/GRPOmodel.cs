namespace ProjectSAP.Models
{
    public class GRPOmodel : SOmodel
    {
        public double? InStock { get; set; } 
        public string? PaymentMethod { get; set; }
        public int? PaymentTerms { get; set; }

        public GRPOmodel()
        {
            CardCode = string.Empty;
            CardName = string.Empty;
            DocNum = string.Empty;
            DocDate = string.Empty;
            DocDueDate = string.Empty;
            DocTotal = string.Empty;
            DocStatus = string.Empty;
            Items = new List<ItemModel>();
            InStock = 0.0;
            PaymentMethod = string.Empty;
            PaymentTerms = 0;
        }
        public GRPOmodel(string cardCode, string cardName, string docNum, string docDate, string docDueDate, string docTotal, string docStatus, List<ItemModel> items, double inStock, string pm, int pt)
        {
            CardCode = cardCode;
            CardName = cardName;
            DocNum = docNum;
            DocDate = docDate;
            DocDueDate = docDueDate;
            DocTotal = docTotal;
            DocStatus = docStatus;
            Items = items;
            InStock = inStock;
            PaymentMethod = pm;
            PaymentTerms = pt;
        }
    }
}
