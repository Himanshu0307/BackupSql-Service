
using System.ServiceProcess;
using System.IO;
using Backup.Models;
using Newtonsoft.Json;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Backup
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class Service1 : ServiceBase
    {

        public LogService log = new LogService();
        public Server server;
        public Thread t1;

        public BackupService backupService;
        private List<TaskScheduler> taskSchedulers = new List<TaskScheduler>();
        public Service1()
        {

            InitializeComponent();

        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);


        private void SetConfig()
        {
            
            if (File.Exists($"{AppContext.BaseDirectory}\\config.json"))
            {

                using (StreamReader streamReader = new StreamReader($"{AppContext.BaseDirectory}\\config.json"))
                {
                    this.server = JsonConvert.DeserializeObject<Server>(streamReader.ReadToEnd());
                    
                    this.log = new LogService(this.server.BaseDir);
                    // log.LogInformation("App Base Directory"+AppContext.BaseDirectory);
                }

            }
            else
            {
                log.LogError("Please Provide Config File");
            }
        }

        public void BackupAll(object sender)
        {
            foreach (Database db in server.Databases)
            {
                this.backupService.ChangeDatabase(db);
                backupService.BackupAndTransfer();
                Thread.Sleep(2000);
            }
        }

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
             this.t1 = new Thread(StartService);
            this.t1.IsBackground = true;
            this.t1.Start();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
           
        }

        public void StartService()
        {
            try
            {
                this.SetConfig();
                this.backupService = new BackupService(server.Databases[0], ref this.server, ref this.log);
                
                foreach (string time in this.server.ScheduleTime)
                {
                    int hour = int.Parse(time.Substring(0, 2));
                    int minute = int.Parse(time.Substring(3, 2));
                    TaskScheduler task = new TaskScheduler(ref log);
                    task.ScheduleTaskAndStart(hour, minute, new TimerCallback(this.BackupAll));
                    this.taskSchedulers.Add(task);
                }
                BackupAll(null);

            }
            catch (System.Exception e)
            {

                log.LogError("Starting Service" + e.ToString());
            }

        }



        protected override void OnStop()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            try
            {
                if (t1.IsAlive)
                    t1.Suspend();
                foreach (TaskScheduler task in taskSchedulers)
                {
                    task.StopTimer();
                }
                backupService = null;
            }
            catch (System.Exception e)
            {
                log.LogError("Stopping Service" + e.ToString());
            }

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
    }
}
