using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Backup.Models;
using System.Data.SqlClient;
using System.IO;


namespace Backup
{
    public class BackupService
    {
        private Database DatabaseInfo;
        private Server ServerInfo;

        private LogService logService ;

        private String ConnectionString;
        private String BackupPath;
        
        public BackupService(Database database, ref Server server,ref LogService logService)
        {
            this.DatabaseInfo = database;
            this.ServerInfo = server;
            this.logService=logService;

            //Create a Backup Directory
            this.BackupPath = this.ServerInfo.BaseDir+"Backup/" + DatabaseInfo.CompanyName + "/";
            // if (!Directory.Exists(BackupPath))
            // {
            //     Directory.CreateDirectory(BackupPath);
            // }
            this.CreateBackupDirectory();
            ConnectionString = $"Data Source={ServerInfo.DatabaseServerName}; UID={ServerInfo.DatabaseUserName}; Password={ServerInfo.DatabasePassword};Database={DatabaseInfo.DatabaseName};";
        }

        public void CreateBackupDirectory(){
            try
            {
                if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }
            }
            catch (System.Exception e)
            {
                
                logService.LogError("Creating Backup Directory"+e.ToString());
            }
        }

      

        public string Backup()
        {
            try
            {
                //Create Directory
                if (!Directory.Exists(BackupPath + DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    Directory.CreateDirectory(BackupPath + DateTime.Now.ToString("yyyy-MM-dd"));
                }
                String finalPath = BackupPath + DateTime.Now.ToString("yyyy-MM-dd") + "/Backup_" + DateTime.Now.ToString("yyyy-MM-dd-ss") + ".bak";
                // finalPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/" + finalPath;
                logService.LogInformation("Final Path for Backup"+finalPath);
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.CommandText = $"BACKUP DATABASE {DatabaseInfo.DatabaseName} TO DISK ='{finalPath}'";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    command.Dispose();
                    logService.LogInformation("Successfully Created Backup");

                    // logService.LogInformation("finalPath"+finalPath);
                }
                return finalPath;
            }
            catch (Exception e)
            {
                logService.LogError("Error Creating Backup:" + e.ToString());
                return null;
            }
        }

        public void BackupNow()
        {
            this.Backup();
        }

        public void BackupAndTransfer()
        {
            FTPService service = new FTPService($"{DatabaseInfo.CompanyName}/{DateTime.Now.ToString("yyyy-MM-dd")}", ref ServerInfo, ref logService);
            string path = this.Backup();
            if (!String.IsNullOrEmpty(path))
                service.TransferBackup(path);
        }
        public void BackupAndTransfer(object state)
        {
            FTPService service = new FTPService($"{DatabaseInfo.CompanyName}/{DateTime.Now.ToString("yyyy-MM-dd")}", ref ServerInfo, ref logService);
            string path = this.Backup();
            if (!String.IsNullOrEmpty(path))
                service.TransferBackup(path);
                
        }


        public void ChangeDatabase(Database db)
        {
            this.DatabaseInfo = db;
           this.BackupPath = this.ServerInfo.BaseDir+"Backup/" + DatabaseInfo.CompanyName + "/";
            if (!Directory.Exists(BackupPath))
            {
                Directory.CreateDirectory(BackupPath);
            }
            ConnectionString = $"Data Source={ServerInfo.DatabaseServerName}; UID={ServerInfo.DatabaseUserName}; Password={ServerInfo.DatabasePassword};Database={DatabaseInfo.DatabaseName};";

        }


    }
}