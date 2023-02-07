using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UploadFilesToBB05022023.MyClasses;

namespace UploadFilesToBB05022023.Pages
{
    public class IndexModel : PageModel
    {
        readonly MyDropboxHelper dropboxHelper = new MyDropboxHelper();

        public void OnGet()
        {
        }

        public IActionResult OnPost(IFormFile file)
        {           
            if (!dropboxHelper.Connected)
            {
                dropboxHelper.ConnectToDB();
            }
            
            return !dropboxHelper.errorOccured && dropboxHelper.UploadFile(file).Result
             ? RedirectToPage("Success")
             : RedirectToPage("Error"); //I'd like to send errorMessage as well, but that'll have to wait.
        }        
    }
}
