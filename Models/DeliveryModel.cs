namespace ProjectSAP.Models
{
    public class DeliveryModel
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string DocTotal { get; set; }
        public string DocStatus { get; set; }
        public List<ItemModel> Items { get; set; }
        public DeliveryModel()
        {
            CardCode = string.Empty;
            CardName = string.Empty;
            DocNum = string.Empty;
            DocDate = string.Empty;
            DocDueDate = string.Empty;
            DocTotal = string.Empty;
            DocStatus = string.Empty;
            Items = new List<ItemModel>();
        }
        public DeliveryModel(string cardCode, string cardName, string docNum, string docDate, string docDueDate, string docTotal, string docStatus, List<ItemModel> items)
        {
            CardCode = cardCode;
            CardName = cardName;
            DocNum = docNum;
            DocDate = docDate;
            DocDueDate = docDueDate;
            DocTotal = docTotal;
            DocStatus = docStatus;
            Items = items;
        }


    }
}
