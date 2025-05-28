using SAPbobsCOM;


namespace ProjectSAP.Services
{
    public class CompanyA_Service
    {

        private Company company1;
        private readonly HttpClient httpClient;

        public CompanyA_Service()
        {
            company1 = new Company();
        }

        public Company GetCompany()
        {
            return company1;
        }

        // Connecting to SAP Business One
        //Company 1
        public bool ConnectToSAP_Company1()
        {

            company1.Server = "WIN-AUNG5VC7PRC";
            company1.DbServerType = BoDataServerTypes.dst_MSSQL2019;
            company1.CompanyDB = "DB_Intercompany01";
            company1.UserName = "manager";
            company1.Password = "1234";

            int connectionResult = company1.Connect();

            return connectionResult == 0;  // If the connection is successful, the result will be 0
        }

        public List<string> GetItemNames1()
        {
            var items = new List<string>();

            if (company1.Connected)
            {
                Recordset recordset = (Recordset)company1.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery("SELECT TOP 10 ItemName FROM OITM");

                while (!recordset.EoF)
                {
                    items.Add(recordset.Fields.Item("ItemName").Value.ToString());
                    recordset.MoveNext();
                }
            }

            return items;
        }

        // Method to create a Purchase Order before a Sales Order
        // Step 1
        public int PurchaseOrder()
        {
            if (company1.Connected)
            {

                Documents purchaseOrder = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseOrders);

                purchaseOrder.CardCode = "500001"; // Example CardCode

                SAPbobsCOM.Recordset oRecordSet = (SAPbobsCOM.Recordset)company1.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecordSet.DoQuery("SELECT t0.ItemCode, t0.ItemName, t1.Price" +
                    " from OITM t0 " +
                    "join ITM1 t1 on t0.ItemCode=t1.ItemCode " +
                    "where t1.PriceList = 2 and t0.ItemCode in ('102','103') "

                    );

                SAPbobsCOM.Recordset oRecordSet1 = (SAPbobsCOM.Recordset)company1.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecordSet1.DoQuery("SELECT DISTINCT t1.DocEntry " +
                    "from POR1 t0 join OPOR t1 on t0.DocEntry=t1.DocEntry " +
                    "where t0.ItemCode in ('102','103') " +
                    "and t1.Canceled = 'N'");

                if (oRecordSet1.RecordCount > 0)
                {
                    Console.WriteLine("There is already a Purchase Order for the specified items.");
                    return -1;
                }

                bool first_line = true;
                while (!oRecordSet.EoF)
                {
                    if (!first_line)
                        purchaseOrder.Lines.Add();

                    purchaseOrder.Lines.ItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
                    purchaseOrder.Lines.Quantity = 3;
                    purchaseOrder.Lines.ItemDescription = oRecordSet.Fields.Item("ItemName").Value.ToString();
                    purchaseOrder.Lines.UnitPrice = oRecordSet.Fields.Item("Price").Value;
                    purchaseOrder.DocDueDate = DateTime.Now.AddDays(5);

                    first_line = false;
                    oRecordSet.MoveNext();
                }

                if (purchaseOrder.Add() != 0)
                {
                    string errorMessage;
                    int errorCode;
                    company1.GetLastError(out errorCode, out errorMessage);
                    Console.WriteLine("Error adding Purchase Order: " + errorCode + " - " + errorMessage);
                    return -1;
                }
                else
                {
                    Console.WriteLine("Purchase Order added successfully.");
                    return Convert.ToInt32(company1.GetNewObjectKey());
                }
            }
            else
            {
                Console.WriteLine("Not connected to SAP Business One.");
                return -1;
            }
        }

        //Step 5
        public bool GoodsReceiptPO(int DocEntryPO)
        {
            Documents po = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseOrders);
            if (po.GetByKey(DocEntryPO) == false)
            {
                Console.WriteLine("Purchase Order not found with DocEntry: " + DocEntryPO);
                return false;
            }
            else
            {
                Documents grpo = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
                grpo.CardCode = po.CardCode;
                grpo.DocDate = DateTime.Now;
                grpo.DocDueDate = DateTime.Now.AddDays(5);
                grpo.TaxDate = DateTime.Now;
                grpo.Comments = "Goods Receipt for Purchase Order " + DocEntryPO;

                for (int i = 0; i < po.Lines.Count; i++)
                {
                    po.Lines.SetCurrentLine(i);
                    grpo.Lines.ItemCode = po.Lines.ItemCode;
                    grpo.Lines.Quantity = po.Lines.Quantity;
                    grpo.Lines.WarehouseCode = po.Lines.WarehouseCode;
                    grpo.Lines.Price = po.Lines.Price;
                    grpo.Lines.TaxCode = po.Lines.TaxCode;
                    grpo.Lines.Add();
                }
                if (grpo.Add() != 0)
                {
                    string errorMessage;
                    int errorCode;
                    company1.GetLastError(out errorCode, out errorMessage);
                    Console.WriteLine("Error adding Goods Receipt PO: " + errorCode + " - " + errorMessage);

                    return false;
                }
                else
                {
                    Console.WriteLine("Goods Receipt PO added successfully.");
                    return true;
                }


            }
        }

        // Step 8
        public bool APInvoice(int DocEntryGRPO)
        {
            Documents grpo = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            if (grpo.GetByKey(DocEntryGRPO) == false)
            {
                Console.WriteLine("Delivery not found with DocEntry: " + DocEntryGRPO);
                return false;
            }

            Documents apInvoice = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
            apInvoice.CardCode = grpo.CardCode;
            apInvoice.DocDate = DateTime.Now;
            apInvoice.DocDueDate = grpo.DocDueDate;
            apInvoice.TaxDate = grpo.TaxDate;

            int lineCount = grpo.Lines.Count;
            for (int i = 0; i < lineCount; i++)
            {
                grpo.Lines.SetCurrentLine(i);
                apInvoice.Lines.ItemCode = grpo.Lines.ItemCode;
                apInvoice.Lines.Quantity = grpo.Lines.Quantity;
                apInvoice.Lines.ItemDescription = grpo.Lines.ItemDescription;
                apInvoice.Lines.UnitPrice = grpo.Lines.UnitPrice;
                apInvoice.Lines.Add();
            }
            int result = apInvoice.Add();
            if (result != 0)
            {
                string errorMessage;
                int errorCode;
                company1.GetLastError(out errorCode, out errorMessage);
                Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
                return false;
            }
            else
            {
                string docEntry = company1.GetNewObjectKey();
                Console.WriteLine("AR Invoice created with DocEntry: " + docEntry);
                return true;
            }
        }
    }


}
