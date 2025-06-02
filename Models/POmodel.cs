using SAPbobsCOM;
using ProjectSAP.Models;

namespace ProjectSAP.Models
{
    public class POmodel
    {
        public string CardCode { get; set; } // Assuming this is the customer code
        public string CardName { get; set; } // Assuming this is the customer name
        public string DocDate { get; set; } // Date of the purchase order
        public string DocDueDate { get; set; } // Due date for the purchase order
        public string DocNum { get; set; } // Document number of the purchase order
        public List<ItemModel> Items { get; set; } = new List<ItemModel>();


        public POmodel( string cardCode, string cardName, string docDate, string docDueDate, string docNum, List<ItemModel> items)
        {
           
            CardCode = cardCode;
            CardName = cardName;
            DocDate = docDate;
            DocDueDate = docDueDate;
            DocNum = docNum;
            Items = items ;
        }

        public POmodel() 
        {
            CardCode = string.Empty;
            CardName = string.Empty;
            DocDate = string.Empty;
            DocDueDate = string.Empty;
            DocNum = string.Empty;
            Items = new List<ItemModel>();

        } 

    }

   
    }
