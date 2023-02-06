using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UploadFilesToBB05022023.Pages
{
    public class IndexModel : PageModel
    {
        static string token =
           "sl.BYWuXnavEGaRs5TVnYL0Qheg5vA41LpxdfkpkHOQO4OrMivSUktotecVsti7VoEUUZVPBOWSg_eOFAislAy0JU9Hx4kW2iIkNHr-mAR7HbmdwjdL_LWydFg8DJFILIrckVvBOcE";

            Dropbox.Api.DropboxClient dbx;
        public bool Connected { get; private set; }
        private bool isConnecting = false;
        private bool errorOccured = false;
        string errorMessage;
       
        public void OnGet()
        {
        }
        public void OnPost(IFormFile file)
        {
            if (errorOccured||isConnecting) 
                return;

            if (!Connected)
            {
                ConnectToDB();
            }
            UploadFile(file);           
        }

        async void ConnectToDB()
        {
            if (!Connected && !isConnecting)
            {
                try
                {
                    isConnecting = true;
                    dbx = new DropboxClient(token);
                    var id = await dbx.Users.GetCurrentAccountAsync();
                }
                catch(Exception ex)
                {
                    errorOccured = true;
                    errorMessage = ex.Message;                                     
                }
                    isConnecting = false;
            }
        }

        async void UploadFile(IFormFile file)
        {
            try
            {
                //incoming file cannot be directly uploaded to Dropbox
                //because they use different File classes...

                var dbPath = "/UploadedFiles/" + file.FileName;
                var localPath = "UploadedFiles\\" + file.FileName;

                await using var temoraryFile = new FileStream(localPath, FileMode.Create);
                await file.CopyToAsync(temoraryFile);
                temoraryFile.Close();

                var mem = new MemoryStream(System.IO.File.ReadAllBytes(localPath));
                await dbx.Files.UploadAsync(dbPath, body: mem);

                if (System.IO.File.Exists(localPath))
                {
                    System.IO.File.Delete(localPath);
                }
            }
            catch(Exception ex)
            {
                errorOccured = true;
                errorMessage = ex.Message;
            }
        }

    }
}
