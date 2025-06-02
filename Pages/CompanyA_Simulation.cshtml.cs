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
        public POmodel PurchaseOrder { get; set; } = new POmodel();
        public SOmodel SalesOrder { get; set; } 

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

            if (ConnectionA != true || ConnectionB!=true)
            {
                Console.WriteLine("Failed to connect to Company A or B in OnGet");
                return;
            }
            ItemNamesA = companyA_Service.GetItemNamesA();

            if(TempData["ValidPO"] != null && TempData.Peek("ValidPO").ToString()=="True")
            {
                TempData.Keep();
                ValidPO = true;
                int result = Convert.ToInt32(TempData.Peek("DocNum"));
                PurchaseOrder = companyA_Service.DiplayPO(result);
            }
            if (TempData["SalesOrderNum"] != null)
            {
                int soNum = Convert.ToInt32(TempData.Peek("SalesOrderNum"));
                SalesOrder = companyB_Service.DisplaySO(soNum);
            }
        }

       
        //public async Task<IActionResult> OnPost()
        //{
        //    var result_PO = -1;
        //    var result_SO = -1;
        //    ConnectionA = companyA_Service.ConnectToSAP_Company1();
        //    ConnectionB = companyB_Service.ConnectToSAP_CompanyB();
        //    if (ConnectionA == false || ConnectionB==false)
        //    {
        //        return new JsonResult(new { success = false, message = "Failed to connect to Company A or Company B." });
        //    }

        //    using var reader = new StreamReader(Request.Body);
        //    var body = await reader.ReadToEndAsync();

        //    var items = JsonSerializer.Deserialize<List<ItemModel>>(body,
        //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //    if (items == null || items.Count == 0)
        //        return new JsonResult(new { success = false, message = "No items received." });

        //    Console.WriteLine(items.Count + " items received for purchase order.");


        //    if (ValidPO == false)
        //    {
        //        //result = companyA_Service.PurchaseOrder(items);
        //        result_PO = 3;
        //        if (result_PO != -1)
        //        {
        //            TempData["ValidPO"] = true;
        //            ValidPO = true;
        //            TempData["DocNum"] = result_PO.ToString();
        //            Console.WriteLine("Purchase order created successfully.");
        //        }
        //        else
        //        {
        //            ValidPO = false;
        //            Console.WriteLine("Failed to create purchase order.");
        //        }
        //        return new JsonResult(new { success = result_PO != -1 });
        //    }
        //    else
        //    {
        //        result_SO = companyB_Service.SalesOrderBasedOnPO(result_PO);
        //        if (result_SO == -1)
        //        {
        //            Console.WriteLine("Failed to create sales order based on purchase order.");
        //            return new JsonResult(new { success = false, message = "Failed to create sales order based on purchase order." });
        //        }
        //        else
        //        {
        //            Console.WriteLine("Sales order created successfully based on purchase order.");
        //        }
        //        return new JsonResult(new { success = result_SO != -1 });
        //    }

           

        //    //return RedirectToPage("/CompanyA_Simulation", new { success = result != -1, validPO = ValidPO, purchaseOrder = PurchaseOrder });
        //}

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
            Console.WriteLine("Purchase order created successfully with DocNum: " + docNum);


            TempData["DocNum"] = docNum;
            TempData["ValidPO"] = true;

            return new JsonResult(new { success = true, docNum });
        }

        public IActionResult OnPostCreateSO()
        {
            ConnectionB = companyB_Service.ConnectToSAP_CompanyB();
            if (!ConnectionB)
                return new JsonResult(new { success = false, message = "Company B connection failed." });

            Console.WriteLine("DocNum = "+TempData["DocNum"]);
            if (!TempData.ContainsKey("DocNum"))
                return new JsonResult(new { success = false, message = "No valid PO found." });

            int docNum = int.Parse(TempData["DocNum"].ToString());

            //var result = companyB_Service.SalesOrderBasedOnPO(docNum);
            var result = 8; // Simulating a successful SO creation for testing purposes
            
            TempData["SalesOrderNum"] = result;
            TempData.Keep(); 

            if (result == -1)
                return new JsonResult(new { success = false, message = "Failed to create SO." });

            return new JsonResult(new { success = true, salesOrderNum = result });
        }


    }


}


