using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Backup
{
    public class TaskScheduler
    {
        public Timer timer;
        public LogService log;

        public TaskScheduler(ref LogService log){
            this.log=log;
        }



        public void StopTimer(){
            timer.Change(Timeout.Infinite,Timeout.Infinite);
            timer.Dispose();
        }

        
        public void ScheduleTaskAndStart(int hour,int minute,TimerCallback callback)
        {
            DateTime now = DateTime.Now;
            DateTime scheduledTime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
            if (now > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }
            int interval = (int)(scheduledTime - now).TotalMilliseconds;
           
            timer=new Timer(callback,null,interval,86400000);
             log.LogInformation("Total Time left to Run:"+interval+" Milliseconds");
        }


        
    }
}