﻿namespace ProjectSAP.Models
{
    public class SOmodel : POmodel
    {  
        public string DocTotal { get; set; }
        public string DocStatus { get; set; }

        public SOmodel() {

            CardCode = string.Empty;
            CardName = string.Empty;
            DocNum = string.Empty;
            DocDate = string.Empty;
            DocDueDate = string.Empty;
            DocTotal = string.Empty;
            DocStatus = string.Empty;
            Items = new List<ItemModel>();
        }
        public SOmodel(string cardCode, string cardName, string docNum, string docDate, string docDueDate, string docTotal, string docStatus, List<ItemModel> items)
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
