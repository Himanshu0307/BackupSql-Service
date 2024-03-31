# BackupSql-Service
Created a Windows Service which can automate the task of taking backup from SQL Server and transfer to target Server

# How to Use:
## 1. Set Up an FTP Server on the Target Machine  
## 2. Add Required information in config.json  
____     - Username: Provide Username for ftp Server,  
____     - Password: Provide Password for ftp Server ,  
____     - DatabaseName: Enter Database name,  
____     - CompanyName: Enter Company Name,  
____     - TargetServerIp": Target Server IP. Eg: "ftp://0.0.0.0:21" ,  
____     - DatabaseServerName: Database Server Name,  
____     - DatabaseUserName: Database Username,  
____     - DatabasePassword: Database Password,  
____     - BaseDir: Base Directory. Eg="D:/"  
### Note: Multipe Database can be provided for Backup.  
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
        
