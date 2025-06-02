using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly SAPConnectionService _sapService;

    public IndexModel(SAPConnectionService sapService)
    {
        _sapService = sapService;
    }

    public bool? Connection1 { get; set; } // notă: bool? ca să putem verifica null
    public bool? Connection2 { get; set; }
    public List<string> ItemNames1 { get; set; }
    public List<string> ItemNames2 { get; set; }

    public void OnGet()
    {
        //ConnectionSuccess = _sapService.ConnectToSAP();
        //ConnectionSuccess = true;

        Connection1 = _sapService.ConnectToSAP_Company1();
        Connection2 = _sapService.ConnectToSAP_Company2();

        //if (Connection1 == true)
        //{
        //    ItemNames1 = _sapService.GetItemNamesA();
        //    Console.WriteLine("Connection to Company 1 successful.");
        //}
        //else
        //{
        //    ItemNames1 = new List<string>();
        //    Console.WriteLine("Connection to Company 1 failed.");
        //}

        //if (Connection2 == true)
        //{
        //    ItemNames2 = _sapService.GetItemNamesB();
        //    Console.WriteLine("Connection to Company 2 successful.");
        //}
        //else
        //{
        //    ItemNames2 = new List<string>();
        //    Console.WriteLine("Connection to Company 2 failed.");
        //}

    }
}
