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
        public bool? Connection { get; set; }
        public bool ValidPO { get; set; } = false;
        public List<ItemModel> ItemNamesA { get; set; } = new List<ItemModel>();
        public POmodel PurchaseOrder { get; set; } = new POmodel();

        public Dictionary<string, int> Items { get; set; }

        public CompanyA_SimulationModel(CompanyB_Service compB, CompanyA_Service companyA_Service)
        {
            this.companyB_Service = compB;
            this.companyA_Service = companyA_Service;
        }

        public void OnGet()
        {
            Connection = companyA_Service.ConnectToSAP_Company1();

            if (Connection == true)
            {
                Console.WriteLine("Connection to Company A successful.");
            }
            else
            {
                Console.WriteLine("Failed to connect to Company A.");
            }
            ItemNamesA = companyA_Service.GetItemNamesA();
        }

       
        public async Task<IActionResult> OnPost()
        {
            Connection = companyA_Service.ConnectToSAP_Company1();
            if (Connection == false)
            {
                return new JsonResult(new { success = false, message = "Failed to connect to Company A." });
            }

            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var items = JsonSerializer.Deserialize<List<ItemModel>>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (items == null || items.Count == 0)
                return new JsonResult(new { success = false, message = "No items received." });

            Console.WriteLine(items.Count + " items received for purchase order.");
            
            //var result = companyA_Service.PurchaseOrder(items);
            var result = 3;

            if(result != -1)
            {
                ValidPO = true;
                PurchaseOrder = companyA_Service.DiplayPO(result);
                Console.WriteLine("Purchase order created successfully.");
            }
            else
            {
                ValidPO = false;
                Console.WriteLine("Failed to create purchase order.");
            }

            return new JsonResult(new { success = result != -1 });
        }
    }
    }
