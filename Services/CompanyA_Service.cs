using SAPbobsCOM;
using ProjectSAP.Models;


namespace ProjectSAP.Services
{
    public class CompanyA_Service
    {

        private Company company1;

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

        public List<ItemModel> GetItemNamesA()
        {
            var items = new List<ItemModel>();

            if (company1.Connected)
            {
                Recordset recordset = (Recordset)company1.GetBusinessObject(BoObjectTypes.BoRecordset);
                recordset.DoQuery("SELECT t0.ItemCode, t0.ItemName, t1.Price" +
                    " from OITM t0 " +
                    "join ITM1 t1 on t0.ItemCode=t1.ItemCode " +
                    "where t1.PriceList = 2 and t0.CardCode = 500001");

                while (!recordset.EoF)
                {
                   items.Add(new ItemModel
                   {
                       ItemCode = recordset.Fields.Item("ItemCode").Value.ToString(),
                       ItemName = recordset.Fields.Item("ItemName").Value.ToString(),
                       Price = Convert.ToDouble(recordset.Fields.Item("Price").Value)
                   });
                    recordset.MoveNext();
                }
            }

            return items;
        }

        // For displaying the purchase order that was made
        public POmodel DiplayPO(int DocEntry)
        {
            POmodel poModel = new POmodel();
            if (company1.Connected)
            {
                Documents purchaseOrder = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseOrders);
                if (purchaseOrder.GetByKey(DocEntry))
                {
                    poModel.DocNum = purchaseOrder.DocNum.ToString();
                    poModel.CardCode = purchaseOrder.CardCode;
                    poModel.CardName = purchaseOrder.CardName;
                    poModel.DocDate = purchaseOrder.DocDate.ToString("yyyy-MM-dd");
                    poModel.DocDueDate = purchaseOrder.DocDueDate.ToString("yyyy-MM-dd");
                    for (int i = 0; i < purchaseOrder.Lines.Count; i++)
                    {
                        purchaseOrder.Lines.SetCurrentLine(i);
                        ItemModel item = new ItemModel
                        {
                            ItemCode = purchaseOrder.Lines.ItemCode,
                            ItemName = purchaseOrder.Lines.ItemDescription,
                            Price = purchaseOrder.Lines.UnitPrice,
                            Quantity = purchaseOrder.Lines.Quantity
                        };
                        poModel.Items.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("Purchase Order not found with DocNum: " + DocEntry);
                }
            }
            else
            {
                Console.WriteLine("Not connected to SAP Business One.");
            }
            return poModel;
        }
        public GRPOmodel DisplayGRPO(int DocEntry)
        {
            GRPOmodel grpo = new GRPOmodel();
            if (company1.Connected)
            {
                Documents goodsReceiptPO = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
                if (goodsReceiptPO.GetByKey(DocEntry))
                {
                    grpo.DocNum = goodsReceiptPO.DocNum.ToString();
                    grpo.CardCode = goodsReceiptPO.CardCode;
                    grpo.CardName = goodsReceiptPO.CardName;
                    grpo.DocDate = goodsReceiptPO.DocDate.ToString("yyyy-MM-dd");
                    grpo.DocDueDate = goodsReceiptPO.DocDueDate.ToString("yyyy-MM-dd");
                    grpo.PaymentTerms = goodsReceiptPO.GroupNumber;
                    grpo.PaymentMethod = goodsReceiptPO.PaymentMethod;
                    grpo.DocTotal = goodsReceiptPO.DocTotal.ToString();
                    grpo.DocStatus = goodsReceiptPO.DocumentStatus.ToString();
                    for (int i = 0; i < goodsReceiptPO.Lines.Count; i++)
                    {
                        goodsReceiptPO.Lines.SetCurrentLine(i);
                        ItemModel item = new ItemModel
                        {
                            ItemCode = goodsReceiptPO.Lines.ItemCode,
                            ItemName = goodsReceiptPO.Lines.ItemDescription,
                            Price = goodsReceiptPO.Lines.UnitPrice,
                            Quantity = goodsReceiptPO.Lines.Quantity,
                            InStock = goodsReceiptPO.Lines.OpenAmount 
                        };
                        grpo.Items.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("Goods Receipt PO not found with DocNum: " + DocEntry);
                }
            }
            else
            {
                Console.WriteLine("Not connected to SAP Business One.");
            }
            return grpo;
        }

        public InvoiceModel DisplayAPInv(int DocEntry)
        {
            InvoiceModel invoice = new InvoiceModel();
            if (company1.Connected)
            {
                Documents apInvoice = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
                if (apInvoice.GetByKey(DocEntry))
                {
                    invoice.DocNum = apInvoice.DocNum.ToString();
                    invoice.CardCode = apInvoice.CardCode;
                    invoice.CardName = apInvoice.CardName;
                    invoice.DocDate = apInvoice.DocDate.ToString("yyyy-MM-dd");
                    invoice.DocDueDate = apInvoice.DocDueDate.ToString("yyyy-MM-dd");
                    invoice.DocTotal = apInvoice.DocTotal.ToString();
                    invoice.DocStatus = apInvoice.DocumentStatus.ToString();
                    for (int i = 0; i < apInvoice.Lines.Count; i++)
                    {
                        apInvoice.Lines.SetCurrentLine(i);
                        ItemModel item = new ItemModel
                        {
                            ItemCode = apInvoice.Lines.ItemCode,
                            ItemName = apInvoice.Lines.ItemDescription,
                            Price = apInvoice.Lines.UnitPrice,
                            Quantity = apInvoice.Lines.Quantity
                        };
                        invoice.Items.Add(item);
                    }
                }
                else
                {
                    Console.WriteLine("AP Invoice not found with DocNum: " + DocEntry);
                }
            }
            else
            {
                Console.WriteLine("Not connected to SAP Business One.");
            }
            return invoice;
        }

        // Method to create a Purchase Order before a Sales Order
        // Step 1
        public int PurchaseOrder(List<ItemModel> items)
        {
            if (company1.Connected)
            {

                Documents purchaseOrder = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseOrders);

                purchaseOrder.CardCode = "500001"; // Example CardCode

                var itemCodes = string.Join(",", items.Select(i => $"'{i.ItemCode}'"));

                SAPbobsCOM.Recordset oRecordSet = (SAPbobsCOM.Recordset)company1.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRecordSet.DoQuery($@"
                        SELECT t0.ItemCode, t0.ItemName, t1.Price
                        FROM OITM t0
                        JOIN ITM1 t1 ON t0.ItemCode = t1.ItemCode
                        WHERE t1.PriceList = 2 AND t0.ItemCode IN ({itemCodes})
                    ");


                bool first_line = true;
                while (!oRecordSet.EoF)
                {
                    var item= items.FirstOrDefault(i => i.ItemCode == oRecordSet.Fields.Item("ItemCode").Value.ToString());
                    if (!first_line)
                        purchaseOrder.Lines.Add();

                    purchaseOrder.Lines.ItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
                    purchaseOrder.Lines.Quantity = item.Quantity;
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
        public int GoodsReceiptPO(int DocEntryPO)
        {
            Documents po = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseOrders);
            if (po.GetByKey(DocEntryPO) == false)
            {
                Console.WriteLine("Purchase Order not found with DocEntry: " + DocEntryPO);
                return -1;
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

                    return -1;
                }
                else
                {
                    Console.WriteLine("Goods Receipt PO added successfully.");
                    return Convert.ToInt32(company1.GetNewObjectKey());
                }


            }
        }

        // Step 8
        public int APInvoice(int DocEntryGRPO)
        {
            Documents grpo = (Documents)company1.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            if (grpo.GetByKey(DocEntryGRPO) == false)
            {
                Console.WriteLine("Delivery not found with DocEntry: " + DocEntryGRPO);
                return -1;
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
                return -1;
            }
            else
            {
                string docEntry = company1.GetNewObjectKey();
                Console.WriteLine("AR Invoice created with DocEntry: " + docEntry);
                return Convert.ToInt32(docEntry);
            }
        }
    }


}
