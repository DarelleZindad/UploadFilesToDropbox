using Dropbox.Api;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UploadFilesToBB05022023.MyClasses
{
    public class MyDropboxHelper
    {
        //temporary token, needs to be changed for every session. Will have to look into OAuth 2
        static string token =
           "sl.BYXKJ6J_drfFeqRb3wewomKkTCwcBtFwFp7ZspDK4HgM2ZW2NXcD5ycaWqVf9b19o528VmO-YHzK7m-ayexl0MAva2nZw9J3W9tSvIVqDkefvUWyvjzNmet-YjmoxAfhhZug61A";

        Dropbox.Api.DropboxClient dbx;
        private bool isConnecting = false;

        public bool Connected { get; private set; }
        public bool errorOccured { get; private set; } = false;
        public string errorMessage { get; private set; }

        public void OnGet()
        {
        }


        public async void ConnectToDB()
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

       public async Task<bool> UploadFile(IFormFile file)
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
