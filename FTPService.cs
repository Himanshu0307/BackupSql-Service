using System;
using Backup.Models;
using System.IO;

using System.Net;


namespace Backup
{
    public class FTPService
    {
        public string RemotePath = "";

        private Server server;
        private LogService logService;

        public FTPService(string remotePath, ref Server server, ref LogService logService)
        {
            RemotePath = remotePath;
            this.server = server;
            this.logService = logService;
        }


        public bool CheckDirectory(string filepath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filepath);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(server.Credential.Username, server.Credential.Password);
                request.KeepAlive = false;
                
                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    
                    // StreamWriter write=new StreamWriter("Backup.log",append:true);
                    // byte[] arr=new byte[response.ContentLength];
                    // response.GetResponseStream().Read(arr,0, (int)response.ContentLength);
                    // write.BaseStream.Write(arr,0,(int)response.ContentLength);
                    // write.Close();
                    // write.Dispose();
                    // logService.LogInformation("sdf");
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void CreateDirectory(string filepath)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filepath);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(server.Credential.Username, server.Credential.Password);
                request.KeepAlive = false;
                using (var status = (FtpWebResponse)request.GetResponse())
                {
                    if(status.StatusCode==FtpStatusCode.PathnameCreated)
                    logService.LogInformation("SuccessFully Created Directory on Server.");
                }
            }
            catch (Exception e)
            {
                logService.LogInformation("Creating Directory on Server." + e.ToString());
            }
        }

        public void TransferBackup(string localBackupFilePath)
        {
            try
            {

                if (!this.CheckDirectory($"{server.TargetServerIp}/{RemotePath}"))
                {
                    this.CreateDirectory($"{server.TargetServerIp}/{RemotePath}");
                }
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{server.TargetServerIp}/{RemotePath}/backup_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.bak");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(server.Credential.Username, server.Credential.Password);

                // Set FTP transfer mode to binary
                request.UseBinary = true;

                // Read the local backup file and upload it to the FTP server
                byte[] fileContents;
                using (FileStream fileStream = new FileStream(localBackupFilePath, FileMode.Open))
                {
                    fileContents = new byte[fileStream.Length];
                    fileStream.Read(fileContents, 0, (int)fileStream.Length);
                }

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                logService.LogInformation($"Upload File Complete, status: {response.StatusDescription}");
                response.Close();
            }
            catch (Exception e)
            {
                logService.LogError(e.ToString());
            }
        }

    }
}