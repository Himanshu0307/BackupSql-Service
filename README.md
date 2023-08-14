# BackupSql-Service
Create a Windows Service which can automate the task of taking backup from SQL Server and transfer to target Server

#How to Use:
1.Create a FTP Server on target machine(where backup is to be delivered). we can confifure using IIS Server  
2. Add Required information in config.json  
     Username: Provide Username for ftp Server,  
     Password: Provide Password for ftp Server ,  
     DatabaseName: Enter Database name,  
     CompanyName: Enter Company Name,  
     TargetServerIp": Target Server IP. Eg: "ftp://0.0.0.0:21" ,  
     DatabaseServerName: Database Server Name,  
     DatabaseUserName: Database Username,  
     DatabasePassword: Database Password,  
     BaseDir: Base Directory. Eg="D:/"  
Note: Multipe Database can be provided for Backup.  
Eg:  "Databases": [  
        {
            "DatabaseName": "Database1",
            "CompanyName": "XYZ"
        },  
        {
            "DatabaseName": "Database2",
            "CompanyName": "XYZ"
        },  
        {
            "DatabaseName": "Database3",
            "CompanyName": "S"
        }  
        ]  


#Install Service
1. Install .NET SDK  
2. Run cmd as admin and change cmd directory to required path  
3. Install Using installutils tool  
        
