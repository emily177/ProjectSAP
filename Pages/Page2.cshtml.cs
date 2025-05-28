using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectSAP.Services;
using SAPbobsCOM;

public class Page2Model : PageModel
{
    private readonly CompanyB_Service companyB_Service;
    private readonly CompanyA_Service companyA_Service;
    public bool? VerifySO { get; set; }
    public bool? VerifySOfromPO { get; set; }
    public bool? VerifyPO { get; set; }
    public bool? VerifyGrPO { get; set; }
    public bool? VerifyDelivery { get; set; }
    public bool? Connection { get; set; }
    public bool? ConnectionA { get; set; }


    public Page2Model(CompanyB_Service compB, CompanyA_Service companyA_Service)
    {
        this.companyB_Service = compB;
        this.companyA_Service = companyA_Service;
    }


    public void OnGet()
    {

        Connection = companyB_Service.ConnectToSAP_CompanyB();
        ConnectionA = companyA_Service.ConnectToSAP_Company1();
        if (Connection == true && ConnectionA == true)
        {
            Console.WriteLine("Connection to Company B successful.");
            //int docEntryDev = companyB_Service.Delivery(6);
            int docEntryDev = 6; 
            if (docEntryDev >0)
            {
                VerifyDelivery = true;
                Console.WriteLine("Delivery created successfully in Company B with DocEntry: " + docEntryDev);
            }
            else
            {
                Console.WriteLine("Failed to create Delivery in Company B.");
            }

            VerifyGrPO = companyA_Service.GoodsReceiptPO(3);
            if (VerifyGrPO == true)
            {
                Console.WriteLine("Goods Receipt PO created successfully in Company A.");
            }
            else
            {
                Console.WriteLine("Failed to create Goods Receipt PO in Company A.");
            }
            //companyB_Service.DeleteSO();
            //VerifySO = companyB_Service.SalesOrder();

        }
        else
        {
            Connection = false;
            Console.WriteLine("Connection failed");
        }
       

    }

    public IActionResult OnPostSales()
    {
        Connection = companyB_Service.ConnectToSAP_CompanyB();
        if (Connection == true)
        {
            Console.WriteLine("Connection to Company B in PostSales successful.");
            VerifySO = companyB_Service.SalesOrder();
        }
        else
        {
            Connection = false;
            Console.WriteLine("Connection failed in PostSales");
        }
        return Page(); 

    }

    public IActionResult OnPostPurchase()
    {
        Connection = companyB_Service.ConnectToSAP_CompanyB();
        ConnectionA = companyA_Service.ConnectToSAP_Company1();
        if (Connection == true && ConnectionA==true)
        {
            Console.WriteLine("Connection to Company A and Company B in PostPurchase successful.");
            //int docEntry = companyA_Service.PurchaseOrder();
            int docEntry = 3;

            if (docEntry>0)
            {
                VerifyPO = true;
                Console.WriteLine("Purchase Order created successfully in Company A.");

                Documents po = (Documents)companyA_Service.GetCompany().GetBusinessObject(BoObjectTypes.oPurchaseOrders);
                if (po.GetByKey(docEntry) == false)
                {
                    Console.WriteLine("Purchase Order not found with DocEntry: " + docEntry);
                }
                else 
                {
                    Console.WriteLine("Purchase Order found with DocEntry: " + docEntry);
                    VerifySOfromPO = companyB_Service.SalesOrderBasedOnPO(po,docEntry);
                }
               

            }
            else
            {
                Console.WriteLine("Failed to create Purchase Order in Company A.");
            }
        }
        else
        {
            Connection = false;
            Console.WriteLine("Connection failed in PostPurchase");
        }
        return Page();

    }
}
