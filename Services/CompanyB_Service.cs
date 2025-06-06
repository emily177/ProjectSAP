using SAPbobsCOM;
using ProjectSAP.Services;
using ProjectSAP.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace ProjectSAP.Services
{
    public class CompanyB_Service
    {

        private Company company2;
        private CompanyA_Service companyA_Service;


        public CompanyB_Service()
        {
            company2 = new Company();
            companyA_Service = new CompanyA_Service();
        }

        public Company GetCompany()
        {
            return company2;
        }


        //Company 2
        public bool ConnectToSAP_CompanyB()
        {
            company2.Server = "WIN-AUNG5VC7PRC";
            company2.DbServerType = BoDataServerTypes.dst_MSSQL2019;
            company2.CompanyDB = "DB_Intercompany02";
            company2.UserName = "manager";
            company2.Password = "1234";

            int connectionResult = company2.Connect();

            if (connectionResult != 0)
            {
                string errorMessage;
                int errorCode;
                company2.GetLastError(out errorCode, out errorMessage);
                Console.WriteLine("Connection failed: " + errorCode + " - " + errorMessage);
                return false;
            }
            else
            {
                //Console.WriteLine("Connection to Company 2 successful.");
                return true;
            }
        }

        // Reading from OITM table (items) in Company 2
        public List<string> GetItemNames2()
        {
            var items = new List<string>();

            if (company2.Connected)
            {
                Recordset recordset = (Recordset)company2.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery("SELECT TOP 10 ItemName FROM OITM");

                while (!recordset.EoF)
                {
                    items.Add(recordset.Fields.Item("ItemName").Value.ToString());
                    recordset.MoveNext();
                }
            }

            return items;
        }

        public SOmodel DisplaySO(int DocEntry)
        {
            SOmodel soModel = new SOmodel();
            if (!company2.Connected)
            {
                Console.WriteLine("Company B is not connected.");
                return soModel;
            }
            Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);
            if (salesOrder.GetByKey(DocEntry))
            {
                soModel.CardCode = salesOrder.CardCode;
                soModel.CardName = salesOrder.CardName;
                soModel.DocNum = salesOrder.DocNum.ToString();
                soModel.DocDate = salesOrder.DocDate.ToString("yyyy-MM-dd");
                soModel.DocDueDate = salesOrder.DocDueDate.ToString("yyyy-MM-dd");
                soModel.DocTotal = salesOrder.DocTotal.ToString();
                soModel.DocStatus = salesOrder.DocumentStatus.ToString();
                for (int i = 0; i < salesOrder.Lines.Count; i++)
                {
                    salesOrder.Lines.SetCurrentLine(i);
                    ItemModel item = new ItemModel
                    {
                        ItemCode = salesOrder.Lines.ItemCode,
                        ItemName = salesOrder.Lines.ItemDescription,
                        Quantity = salesOrder.Lines.Quantity,
                        Price = salesOrder.Lines.UnitPrice
                        
                    };
                    soModel.Items.Add(item);
                }
            }
            else
            {
                Console.WriteLine("Sales Order not found with DocEntry: " + DocEntry);
            }
            return soModel;

        }
        // Display Delivery Note based on DocEntry
        public DeliveryModel DisplayDelivery(int DocEntry)
        {
            DeliveryModel deliveryModel = new DeliveryModel();

            if (!company2.Connected)
            {
                Console.WriteLine("Company B is not connected.");
                return deliveryModel;
            }
            Documents delivery = (Documents)company2.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
            if (delivery.GetByKey(DocEntry))
            {
                deliveryModel.CardCode = delivery.CardCode;
                deliveryModel.CardName = delivery.CardName;
                deliveryModel.DocNum = delivery.DocNum.ToString();
                deliveryModel.DocDate = delivery.DocDate.ToString("yyyy-MM-dd");
                deliveryModel.DocDueDate = delivery.DocDueDate.ToString("yyyy-MM-dd");
                deliveryModel.DocTotal = delivery.DocTotal.ToString();
                deliveryModel.DocStatus = delivery.DocumentStatus.ToString();
                for (int i = 0; i < delivery.Lines.Count; i++)
                {
                    delivery.Lines.SetCurrentLine(i);
                    ItemModel item = new ItemModel
                    {
                        ItemCode = delivery.Lines.ItemCode,
                        ItemName = delivery.Lines.ItemDescription,
                        Quantity = delivery.Lines.Quantity,
                        Price = delivery.Lines.UnitPrice
                    };
                    deliveryModel.Items.Add(item);
                }
            }
            else
            {
                Console.WriteLine("Delivery Note not found with DocEntry: " + DocEntry);
            }
            return deliveryModel;

        }
        // Display AR Invoice based on DocEntry

        public InvoiceModel DisplayARInv(int DocEntry)
        {

            InvoiceModel ARInv = new InvoiceModel();
            if (!company2.Connected)
            {
                Console.WriteLine("Company B is not connected.");
                return ARInv;
            }

            Documents arInvoice = (Documents)company2.GetBusinessObject(BoObjectTypes.oInvoices);

            if (arInvoice.GetByKey(DocEntry))
            {
                ARInv.CardCode = arInvoice.CardCode;
                ARInv.CardName = arInvoice.CardName;
                ARInv.DocNum = arInvoice.DocNum.ToString();
                ARInv.DocDate = arInvoice.DocDate.ToString("yyyy-MM-dd");
                ARInv.DocDueDate = arInvoice.DocDueDate.ToString("yyyy-MM-dd");
                ARInv.DocTotal = arInvoice.DocTotal.ToString();
                
                for (int i = 0; i < arInvoice.Lines.Count; i++)
                {
                    arInvoice.Lines.SetCurrentLine(i);
                    ItemModel item = new ItemModel
                    {
                        ItemCode = arInvoice.Lines.ItemCode,
                        ItemName = arInvoice.Lines.ItemDescription,
                        Quantity = arInvoice.Lines.Quantity,
                        Price = arInvoice.Lines.UnitPrice
                    };
                    ARInv.Items.Add(item);
                }
            }
            else
            {
                Console.WriteLine("AR Invoice not found with DocEntry: " + DocEntry);
            }


            return ARInv;
        }

        //Step 2
        public int SalesOrderBasedOnPO( int poDocEntry)
        {
            companyA_Service.ConnectToSAP_Company1();
            if (!companyA_Service.GetCompany().Connected)
            {
                Console.WriteLine("Failed to connect to Company A in SO.");
                return -1;
            }
            Documents purchaseOrder = (Documents)companyA_Service.GetCompany().GetBusinessObject(BoObjectTypes.oPurchaseOrders);

            if (purchaseOrder.GetByKey(poDocEntry) == false)
            {
                Console.WriteLine("Purchase Order not found with DocEntry: " + poDocEntry);
                return -1;
            }
            Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);

            salesOrder.CardCode = "100001";
            salesOrder.DocDate = DateTime.Now;
            salesOrder.DocDueDate = purchaseOrder.DocDueDate;
            salesOrder.TaxDate = purchaseOrder.TaxDate;

            int lineCount = purchaseOrder.Lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                purchaseOrder.Lines.SetCurrentLine(i);
                salesOrder.Lines.ItemCode = purchaseOrder.Lines.ItemCode;
                salesOrder.Lines.Quantity = purchaseOrder.Lines.Quantity;
                salesOrder.Lines.ItemDescription = purchaseOrder.Lines.ItemDescription;
                salesOrder.Lines.UnitPrice = purchaseOrder.Lines.UnitPrice;

                salesOrder.Lines.Add();

            }
            int result = salesOrder.Add();
            if (result != 0)
            {
                string errorMessage;
                int errorCode;
                company2.GetLastError(out errorCode, out errorMessage);
                Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
                return -1;
            }
            else
            {
                string docEntry = company2.GetNewObjectKey();
                Console.WriteLine("Sales Order created with DocEntry: " + docEntry);
                return Convert.ToInt32(docEntry);
            }

        }


        //Step 4
        public int Delivery(int DocEntrySO)
        {
            Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);
            if (salesOrder.GetByKey(DocEntrySO) == false)
            {
                Console.WriteLine("Delivery not found with DocEntry: " + DocEntrySO);
                return -1;
            }
            Documents delivery = (Documents)company2.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
            delivery.CardCode = salesOrder.CardCode;
            delivery.DocDate = DateTime.Now;
            delivery.DocDueDate = salesOrder.DocDueDate;
            delivery.TaxDate = salesOrder.TaxDate;

            int lineCount = salesOrder.Lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                salesOrder.Lines.SetCurrentLine(i);
                delivery.Lines.ItemCode = salesOrder.Lines.ItemCode;
                delivery.Lines.Quantity = salesOrder.Lines.Quantity;
                delivery.Lines.ItemDescription = salesOrder.Lines.ItemDescription;
                delivery.Lines.UnitPrice = salesOrder.Lines.UnitPrice;
                delivery.Lines.Add();
            }
            int result = delivery.Add();
            if (result != 0)
            {
                string errorMessage;
                int errorCode;
                company2.GetLastError(out errorCode, out errorMessage);
                Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
                return -1;
            }
            else
            {
                string docEntry = company2.GetNewObjectKey();
                Console.WriteLine("Delivery created with DocEntry: " + docEntry);
                return Convert.ToInt32(docEntry);
            }
        }

        // Step 7
        public int ARInvoice(int DocEntryDelivery)
        {
            Documents delivery = (Documents)company2.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
            if (delivery.GetByKey(DocEntryDelivery) == false)
            {
                Console.WriteLine("Delivery not found with DocEntry: " + DocEntryDelivery);
                return -1;
            }

            Documents arInvoice = (Documents)company2.GetBusinessObject(BoObjectTypes.oInvoices);
            arInvoice.CardCode = delivery.CardCode;
            arInvoice.DocDate = DateTime.Now;
            arInvoice.DocDueDate = delivery.DocDueDate;
            arInvoice.TaxDate = delivery.TaxDate;

            int lineCount = delivery.Lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                delivery.Lines.SetCurrentLine(i);
                arInvoice.Lines.ItemCode = delivery.Lines.ItemCode;
                arInvoice.Lines.Quantity = delivery.Lines.Quantity;
                arInvoice.Lines.ItemDescription = delivery.Lines.ItemDescription;
                arInvoice.Lines.UnitPrice = delivery.Lines.UnitPrice;
                arInvoice.Lines.Add();
            }
            int result = arInvoice.Add();
            if (result != 0)
            {
                string errorMessage;
                int errorCode;
                company2.GetLastError(out errorCode, out errorMessage);
                Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
                return -1;
            }
            else
            {
                string docEntry = company2.GetNewObjectKey();
                Console.WriteLine("AR Invoice created with DocEntry: " + docEntry);
                return Convert.ToInt32(docEntry);
            }
        }

   
    }


}
