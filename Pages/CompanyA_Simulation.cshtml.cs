using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectSAP.Services;
using ProjectSAP.Models;
using SAPbobsCOM;
using System.Text.Json;

namespace ProjectSAP.Pages
{
    [IgnoreAntiforgeryToken]
    public class CompanyA_SimulationModel : PageModel
    {
        private readonly CompanyB_Service companyB_Service;
        private readonly CompanyA_Service companyA_Service;
        public bool ConnectionA { get; set; }
        public bool ConnectionB { get; set; }
        public bool ValidPO { get; set; } = false;
        public List<ItemModel> ItemNamesA { get; set; } = new List<ItemModel>();
        public POmodel PurchaseOrder { get; set; }
        public SOmodel SalesOrder { get; set; }
        public DeliveryModel Delivery { get; set; }
        public GRPOmodel GRPO { get; set; }
        public InvoiceModel ARInvoice { get; set; }
        public InvoiceModel APInvoice { get; set; }

        public Dictionary<string, int> Items { get; set; }

        public CompanyA_SimulationModel(CompanyB_Service compB, CompanyA_Service companyA_Service)
        {
            this.companyB_Service = compB;
            this.companyA_Service = companyA_Service;
        }

        public void OnGet()
        {
            ConnectionA = companyA_Service.ConnectToSAP_Company1();
            ConnectionB = companyB_Service.ConnectToSAP_CompanyB();

            if (ConnectionA != true || ConnectionB != true)
            {
                Console.WriteLine("Failed to connect to Company A or B in OnGet");
                return;
            }
            ItemNamesA = companyA_Service.GetItemNamesA();

            if (TempData["ValidPO"] != null && TempData.Peek("ValidPO").ToString() == "True")
            {
                TempData.Keep();
                ValidPO = true;
                int result = Convert.ToInt32(TempData.Peek("PO_Num"));
                PurchaseOrder = companyA_Service.DiplayPO(result);
            }
            if (TempData["SalesOrderNum"] != null)
            {

                int soNum = Convert.ToInt32(TempData.Peek("SalesOrderNum"));
                SalesOrder = companyB_Service.DisplaySO(soNum);
            }
            if (TempData["DeliveryNum"] != null)
            {
                int dlNum = Convert.ToInt32(TempData.Peek("DeliveryNum"));
                SalesOrder = null;
                Delivery = companyB_Service.DisplayDelivery(dlNum);
            }
            if (TempData["GRPO_Num"] != null)
            {
                int grpoNum = Convert.ToInt32(TempData.Peek("GRPO_Num"));
                PurchaseOrder = null;
                GRPO = companyA_Service.DisplayGRPO(grpoNum);
            }
            if (TempData["ARInv_Num"] != null)
            {
                int arInvNum = Convert.ToInt32(TempData.Peek("ARInv_Num"));
                Delivery = null;
                ARInvoice = companyB_Service.DisplayARInv(arInvNum);
            }
            if (TempData["APInv_Num"] != null)
            {
                int apInvNum = Convert.ToInt32(TempData.Peek("APInv_Num"));
                GRPO = null;
                APInvoice = companyA_Service.DisplayAPInv(apInvNum);
            }
        }

        public async Task<IActionResult> OnPostCreatePO()
        {
            ConnectionA = companyA_Service.ConnectToSAP_Company1();
            if (!ConnectionA)
                return new JsonResult(new { success = false, message = "Company A connection failed." });

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var items = JsonSerializer.Deserialize<List<ItemModel>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (items == null || items.Count == 0)
                return new JsonResult(new { success = false, message = "No items received." });

            //var docNum = companyA_Service.PurchaseOrder(items); 
            var docNum = 3;
            if (docNum == -1)
            {
                Console.WriteLine("Failed to create purchase order.");
                return new JsonResult(new { success = false, message = "Failed to create PO" });
            }
            Console.WriteLine("Purchase order created successfully with PO_Num: " + docNum);


            TempData["PO_Num"] = docNum;
            TempData["ValidPO"] = true;

            return new JsonResult(new { success = true, docNum });
        }

        public IActionResult OnPostCreateSO()
        {
            ConnectionB = companyB_Service.ConnectToSAP_CompanyB();
            if (!ConnectionB)
                return new JsonResult(new { success = false, message = "Company B connection failed." });

            Console.WriteLine("PO_Num = " + TempData["PO_Num"]);
            if (!TempData.ContainsKey("PO_Num"))
                return new JsonResult(new { success = false, message = "No valid PO found." });

            int docNum = int.Parse(TempData["PO_Num"].ToString());

            //var result = companyB_Service.SalesOrderBasedOnPO(docNum);
            var result = 8;

            TempData["SalesOrderNum"] = result;
            TempData.Keep();

            if (result == -1)
                return new JsonResult(new { success = false, message = "Failed to create SO." });

            return new JsonResult(new { success = true, salesOrderNum = result });
        }

        public IActionResult OnPostCreateDl()
        {
            ConnectionB = companyB_Service.ConnectToSAP_CompanyB();
            if (!ConnectionB)
                return new JsonResult(new { success = false, message = "Company B connection failed." });
            if (!TempData.ContainsKey("SalesOrderNum"))
                return new JsonResult(new { success = false, message = "No valid SO found." });
            int salesOrderNum = int.Parse(TempData["SalesOrderNum"].ToString());

            Console.WriteLine("SalesOrderNum = " + salesOrderNum);

            //var result = companyB_Service.CreateDelivery(salesOrderNum);
            var result = 1;

            TempData["DeliveryNum"] = result;
            TempData.Keep();

            if (result == -1)
                return new JsonResult(new { success = false, message = "Failed to create Delivery." });
            return new JsonResult(new { success = true, deliveryNum = result });


        }

        public IActionResult OnPostCreateGRPO()
        {
            // We will make a GRPO document based on the PO created in Company A
            ConnectionA = companyA_Service.ConnectToSAP_Company1();
            if (!ConnectionA)
                return new JsonResult(new { success = false, message = "Company A connection failed." });

            if (!TempData.ContainsKey("PO_Num"))
                return new JsonResult(new { success = false, message = "No valid PO found." });

            int poNum = int.Parse(TempData["PO_Num"].ToString());
            Console.WriteLine("PO_Num = " + poNum);
            //var result = companyA_Service.GoodsReceiptPO(poNum);
            var result = 3; // Simulating a successful GRPO creation for testing purposes
            if (result == -1)
            {
                Console.WriteLine("Failed to create GRPO.");
                return new JsonResult(new { success = false, message = "Failed to create GRPO." });
            }

            Console.WriteLine("GRPO created successfully with GRPO_Num: " + result);

            TempData["GRPO_Num"] = result;
            TempData.Keep();

            return new JsonResult(new { success = true, message = "GRPO creation initiated." });

        }

        public IActionResult OnPostCreateARInv()
        {

            ConnectionB = companyB_Service.ConnectToSAP_CompanyB();
            if (!ConnectionB)
                return new JsonResult(new { success = false, message = "Company B connection failed." });
            if (!TempData.ContainsKey("DeliveryNum"))
                return new JsonResult(new { success = false, message = "No valid Delivery found." });
            int deliveryNum = int.Parse(TempData["DeliveryNum"].ToString());
            Console.WriteLine("DeliveryNum = " + deliveryNum);

            //var result = companyB_Service.CreateARInvoice(deliveryNum);
            var result = 1; // Simulating a successful AR Invoice creation for testing purposes
            if (result == -1)
            {
                Console.WriteLine("Failed to create AR Invoice.");
                return new JsonResult(new { success = false, message = "Failed to create AR Invoice." });
            }
            Console.WriteLine("AR Invoice created successfully with ARInv_Num: " + result);

            TempData["ARInv_Num"] = result;
            TempData.Keep();
            return new JsonResult(new { success = true, arInvNum = result });
        }

        public IActionResult OnPostCreateAPInv()
        {
            ConnectionA = companyA_Service.ConnectToSAP_Company1();
            if (!ConnectionA)
                return new JsonResult(new { success = false, message = "Company A connection failed." });
            if (!TempData.ContainsKey("GRPO_Num"))
                return new JsonResult(new { success = false, message = "No valid GRPO found." });
            int grpoNum = int.Parse(TempData["GRPO_Num"].ToString());
            Console.WriteLine("GRPO_Num = " + grpoNum);
            //var result = companyA_Service.CreateAPInvoice(grpoNum);
            var result = 1; // Simulating a successful AP Invoice creation for testing purposes
            if (result == -1)
            {
                Console.WriteLine("Failed to create AP Invoice.");
                return new JsonResult(new { success = false, message = "Failed to create AP Invoice." });
            }
            Console.WriteLine("AP Invoice created successfully with APInv_Num: " + result);
            TempData["APInv_Num"] = result;
            TempData.Keep();
            return new JsonResult(new { success = true, apInvNum = result });
        }
    }


}


