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
        //temporary token, needs to be changed for every session. Will have to look into OAuth 2
        static string token =
           "sl.BYXKJ6J_drfFeqRb3wewomKkTCwcBtFwFp7ZspDK4HgM2ZW2NXcD5ycaWqVf9b19o528VmO-YHzK7m-ayexl0MAva2nZw9J3W9tSvIVqDkefvUWyvjzNmet-YjmoxAfhhZug61A";

        Dropbox.Api.DropboxClient dbx;

        private bool Connected = false;
        private bool isConnecting = false;
        private bool errorOccured = false;
        string errorMessage;
       
        public void OnGet()
        {
        }

        public IActionResult OnPost(IFormFile file)
        {           
            if (!Connected)
            {
                ConnectToDB();
            }
            
            return !errorOccured && UploadFile(file).Result
             ? RedirectToPage("Success")
             : RedirectToPage("Error"); //I'd like to send errorMessage as well, but that'll have to wait.
        }

        async void ConnectToDB()
        {
            if (!Connected && !isConnecting)
            {
                isConnecting = true;
                try
                {
                    dbx = new DropboxClient(token);
                    var id = await dbx.Users.GetCurrentAccountAsync();
                }
                catch (Exception ex)
                {
                    errorOccured = true;
                    errorMessage = ex.Message;
                }

                isConnecting = false;
            }
        }

        async Task<bool> UploadFile(IFormFile file)
        {
            //file can't be directly loaded to DB bc they use different types of file
            bool success = false;

            if (file is null)
            {
                errorMessage = "no file chosen";
            }
            else
            {
                try
                {
                    var dbPath = "/UploadedFiles/" + file.FileName;
                    var localPath = "UploadedFiles\\" + file.FileName;

                    //copy file to folder
                    await using var temoraryFile = new FileStream(localPath, FileMode.Create);
                    await file.CopyToAsync(temoraryFile);
                    temoraryFile.Close();

                    //upload file to Dropbox
                    var mem = new MemoryStream(System.IO.File.ReadAllBytes(localPath));
                    await dbx.Files.UploadAsync(dbPath, body: mem);

                    //delete file from folder
                    if (System.IO.File.Exists(localPath))
                    {
                        System.IO.File.Delete(localPath);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }

            return success;
        }       
    }
}
