namespace ProjectSAP.Models
{
    public class InvoiceModel : SOmodel
    {
        public InvoiceModel()
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
        public InvoiceModel(string cardCode, string cardName, string docNum, string docDate, string docDueDate, string docTotal, string docStatus, List<ItemModel> items)
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
