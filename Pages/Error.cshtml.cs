using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UploadFilesToBB05022023.Pages
{
    public class ErrorModel : PageModel
    {
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
           
        }

        public ErrorModel(string errorMessage)
        {
 ErrorMessage = errorMessage;
        }
    }
}
