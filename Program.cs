using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Backup.Models;

namespace Backup
{
    internal static class Program
    {

        static void Installer(string[] args)
        {
            const string ServiceName = "SQLFinnauxBackup";

            if (args.Length == 1)
            {
                try
                {
                    string executablePath =
                        Path.Combine(AppContext.BaseDirectory, "Backup.exe");

                    if (args[0] is "/Install")
                    {

                        try
                        {
                            Process install = new Process();
                            install.StartInfo.FileName = "cmd.exe";
                            install.StartInfo.CreateNoWindow = true;
                            install.Start();

                            install.StandardInput.Write($"sc create {ServiceName} binPath={executablePath} start=auto");
                            // install.StandardInput.Write($"sc start {ServiceName}");
                            install.StandardInput.Flush();
                            install.StandardInput.Close();
                            install.WaitForExit();
                            install.Dispose();
                        }
                        catch (Exception e)
                        {

                        }

                    }
                    else if (args[0] is "/Uninstall")
                    {

                        try
                        {
                            Process install = new Process();
                            install.StartInfo.FileName = "cmd.exe";
                            install.StartInfo.CreateNoWindow = true;
                            install.Start();

                            install.StandardInput.Write($"sc stop {ServiceName}");
                            install.StandardInput.Write($"sc delete {ServiceName}");
                            install.StandardInput.Flush();
                            install.StandardInput.Close();
                            install.WaitForExit();
                            install.Dispose();
                        }
                        catch (Exception e)
                        {

                        }


                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            }
        }





        static void Main(string[] args)
        {

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }





    }
}
