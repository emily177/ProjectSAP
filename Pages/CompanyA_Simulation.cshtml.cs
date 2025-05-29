using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectSAP.Services;
using SAPbobsCOM;

namespace ProjectSAP.Pages
{
    public class CompanyA_SimulationModel : PageModel
    {
        private readonly CompanyB_Service companyB_Service;
        private readonly CompanyA_Service companyA_Service;
        public bool? VerifySO { get; set; }
        public bool? VerifySOfromPO { get; set; }
        public bool? VerifyPO { get; set; }
        public bool? VerifyGrPO { get; set; }
        public bool? VerifyDelivery { get; set; }
        public bool? VerifyARInv { get; set; }
        public bool? VerifyAPInv { get; set; }
        public bool? Connection { get; set; }
        public bool? ConnectionA { get; set; }
        public List<string> ItemNamesA { get; set; } = new List<string>();

        public CompanyA_SimulationModel(CompanyB_Service compB, CompanyA_Service companyA_Service)
        {
            this.companyB_Service = compB;
            this.companyA_Service = companyA_Service;
        }

        public void OnGet()
        {
            ItemNamesA = companyA_Service.GetItemNamesA();
        }
    }
}
