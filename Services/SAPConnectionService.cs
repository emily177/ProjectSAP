using SAPbobsCOM;

public class SAPConnectionService
{
    private Company company1;
    private Company company2;
   // private readonly HttpClient httpClient;

    public SAPConnectionService()
    {
        company1 = new Company();
        company2 = new Company();
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
    //Company 2
    public bool ConnectToSAP_Company2()
    {
        company2.Server = "WIN-AUNG5VC7PRC";
        company2.DbServerType = BoDataServerTypes.dst_MSSQL2019;
        company2.CompanyDB = "DB_Intercompany02";
        company2.UserName = "manager";
        company2.Password = "1234";

        int connectionResult = company2.Connect();

        return connectionResult == 0;  
    }

    // Reading from OITM table (items) in Company 2
    public List<string> GetItemNamesB()
    {
        var items = new List<string>();

        if (company2.Connected)
        {
            Recordset recordset = (Recordset) company2.GetBusinessObject(BoObjectTypes.BoRecordset);
            recordset.DoQuery("SELECT TOP 10 ItemName FROM OITM");

            while (!recordset.EoF)
            {
                items.Add(recordset.Fields.Item("ItemName").Value.ToString());
                recordset.MoveNext();
            }
        }

        return items;
    }
    public List<string> GetItemNamesA()
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
    //Compania B (vanzatorul) creeaza un SalesOrder
    public void SalesOrder()
    {
        Documents salesOrder = (Documents)company2.GetBusinessObject(BoObjectTypes.oOrders);

        SAPbobsCOM.Recordset oRecordSet = (SAPbobsCOM.Recordset)company2.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        oRecordSet.DoQuery("Select * from OITM where ItemCode in ('102','103')");
        //Datele pentru Sales Order

        while(!oRecordSet.EoF)
        {
            salesOrder.Lines.ItemCode = oRecordSet.Fields.Item("ItemCode").Value.ToString();
            salesOrder.Lines.Quantity = 3;
            salesOrder.Lines.ItemDescription = oRecordSet.Fields.Item("ItemName").Value.ToString();
            salesOrder.Lines.Price = oRecordSet.Fields.Item("Price").Value;
            salesOrder.Lines.Add();
            oRecordSet.MoveNext();
        }

        int RezultOfAdd = salesOrder.Add();
        if(RezultOfAdd != 0)
        {
            string errorMessage;
            int errorCode;
            company2.GetLastError(out errorCode, out errorMessage);
            Console.WriteLine("Error: " + errorCode + " - " + errorMessage);
        }
        else
        {
            string docEntry;
            docEntry = company2.GetNewObjectKey();
            Console.WriteLine("Sales Order created with DocEntry: " + docEntry);
        }

    }
}
