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

       // private readonly HttpClient httpClient;

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

        //Compania B (vanzatorul) creeaza un SalesOrder
        //public bool SalesOrder()
        //{
        //    Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);
        //    salesOrder.CardCode = "100001";

        //    SAPbobsCOM.Recordset oRecordSet2 = (SAPbobsCOM.Recordset)company2.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

        //    oRecordSet2.DoQuery(
        //       "SELECT DISTINCT T0.DocEntry" +
        //       " FROM ORDR T0" +
        //       " JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry" +
        //       "       WHERE T0.CardCode = '100001'" +
        //       "       AND T1.ItemCode in ('102','103')" +
        //       "       AND T0.Canceled = 'N'");

        //    if (oRecordSet2.RecordCount > 0)
        //    {
        //        Console.WriteLine("Sales Order already exists for the given CardCode and ItemCode.");
        //        return false;
        //    }

        //    SAPbobsCOM.Recordset oRecordSet = (SAPbobsCOM.Recordset)company2.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        //    oRecordSet.DoQuery("SELECT t0.ItemCode, t0.ItemName, t1.Price" +
        //        " from OITM t0 " +
        //        "join ITM1 t1 on t0.ItemCode=t1.ItemCode " +
        //        "where t1.PriceList = 2 and t0.ItemCode in ('102','103')");



        //    //Datele pentru Sales Order
        //    if (oRecordSet.RecordCount == 0)
        //    {
        //        Console.WriteLine("No items found for the given query.");
        //        return false;
        //    }

        //    bool first_line = true;
        //    while (!oRecordSet.EoF)
        //    {
        //        if (!first_line)
        //            salesOrder.Lines.Add();

        //        salesOrder.Lines.ItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
        //        salesOrder.Lines.Quantity = 3;
        //        salesOrder.Lines.ItemDescription = oRecordSet.Fields.Item("ItemName").Value.ToString();
        //        salesOrder.Lines.UnitPrice = oRecordSet.Fields.Item("Price").Value;
        //        salesOrder.DocDueDate = DateTime.Now.AddDays(5);

        //        first_line = false;
        //        oRecordSet.MoveNext();
        //    }
        //    Console.WriteLine("Sales Order Lines added successfully.");
        //    int RezultOfAdd = salesOrder.Add();
        //    if (RezultOfAdd != 0)
        //    {
        //        string errorMessage;
        //        int errorCode;
        //        company2.GetLastError(out errorCode, out errorMessage);
        //        Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
        //        return false;
        //    }
        //    else
        //    {
        //        string docEntry;
        //        docEntry = company2.GetNewObjectKey();
        //        Console.WriteLine("Sales Order created with DocEntry: " + docEntry);
        //        return true;
        //    }

        //}

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

        public void DeleteSO()
        {
            Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);
            SAPbobsCOM.Recordset oRecordSet2 = (SAPbobsCOM.Recordset)company2.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            oRecordSet2.DoQuery(
               "SELECT DISTINCT T0.DocEntry" +
               " FROM ORDR T0" +
               " JOIN RDR1 T1 ON T0.DocEntry = T1.DocEntry" +
               "       WHERE T0.CardCode = '100001'" +
               "       AND T1.ItemCode in ('102','103')" +
               "       AND T0.Canceled = 'N'");

            //Verify if the Sales Order already exists for the given CardCode and ItemCode
            if (oRecordSet2.RecordCount > 0)
            {
                for (int i = 0; i < oRecordSet2.RecordCount; i++)
                {
                    string docEntry = oRecordSet2.Fields.Item("DocEntry").Value.ToString();
                    Console.WriteLine("Sales Order already exists with DocEntry: " + docEntry);

                    if (salesOrder.GetByKey(Convert.ToInt32(docEntry)))
                    {
                        int removeResult = salesOrder.Remove();
                        if (removeResult != 0)
                        {
                            company2.GetLastError(out int errorCode, out string errorMessage);
                            Console.WriteLine($"Eroare la ștergere: {errorCode} - {errorMessage}");
                        }
                        else
                        {
                            Console.WriteLine($"Sales Order cu DocEntry {docEntry} a fost șters.");
                        }

                    }

                    oRecordSet2.MoveNext();
                }
            }
            else
            {
                Console.WriteLine("No Sales Order found for the given CardCode and ItemCode.");
            }
        }
    }


}
