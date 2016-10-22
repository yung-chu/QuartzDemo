using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Quartz.Impl;
using Quartz.Xml;
using Quartz.Simpl;

namespace QuartzPullDataWindowService
{
    class Program
    {
        static void Main(string[] args)
        {

            HostFactory.Run(x =>                                 //1
            {
                x.Service<ServiceRunner>();
                x.RunAsLocalSystem();                            //2
                //3

                x.SetDescription("htmlTest");                    //4
                x.SetDisplayName("htmlTest");                    //5
                x.SetServiceName("htmlTest");                    //6
            });


            Console.ReadKey();

        }








        //配置Quartz
        public void ConfigQuartz(IScheduler sched)
        {

            XMLSchedulingDataProcessor processor = new XMLSchedulingDataProcessor(new SimpleTypeLoadHelper());


            //1.首先创建一个作业调度池
            // ISchedulerFactory schedf = new StdSchedulerFactory();
            // IScheduler sched = schedf.GetScheduler();


            //2.创建出来一个具体的作业
            IJobDetail job = JobBuilder.Create<JobDemo>().Build();


            //3.创建并配置一个触发器
            ISimpleTrigger trigger = WithIntervalInSeconds();


            //4.加入作业调度池中
            sched.ScheduleJob(job, trigger);


            //5.开始运行
            sched.Start();

        }



        //多少秒执行一次
        public static ISimpleTrigger WithIntervalInSeconds()
        {
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
          .WithSimpleSchedule(x => x.WithIntervalInSeconds(3)   //WithIntervalInSeconds  每多少秒执行一次
              .RepeatForever()).Build();


            return trigger;
        }


    }



    //配置window 服务
    public class ServiceRunner : ServiceControl, ServiceSuspend
    {

        private readonly IScheduler scheduler;
        ISchedulerFactory schedf = new StdSchedulerFactory();

        public ServiceRunner()
        {
            scheduler = schedf.GetScheduler();
            new Program().ConfigQuartz(scheduler);
        }

        public bool Start(HostControl hostControl)
        {
            scheduler.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            scheduler.Shutdown(false);
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            scheduler.ResumeAll();
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            scheduler.PauseAll();
            return true;
        }

    }
}
