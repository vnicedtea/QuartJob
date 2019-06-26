using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;
using Quartz.Impl.Matchers;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QuartzJob
{
    public sealed class QuarztSingleton
    {
        private QuarztSingleton() { }
        private static readonly Lazy<QuarztSingleton> lazy = new Lazy<QuarztSingleton>(() => new QuarztSingleton());
        public static QuarztSingleton Instance
        {
            get { return lazy.Value; }
        }
        public static readonly IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

        #region private method         
        private static IJobDetail addJob<T>(string nameJob, string nameGroup) where T : IJob
        {
            return JobBuilder.Create<T>()
                    .WithIdentity(nameJob, nameGroup)
                    .Build();
        }
        private static ITrigger addTrigger(string nameTrigger, string nameGroupnameTrigger, int second)
        {
            return TriggerBuilder.Create()
                     .WithIdentity(nameTrigger, nameGroupnameTrigger)
                     .WithDailyTimeIntervalSchedule(s => s
                            .WithIntervalInSeconds(second)
                            .OnEveryDay().
                            StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0)))
                     .Build();
        }
        public static bool addCron<T>(string nameJob, string nameGroup, string nameTrigger, string nameGroupnameTrigger, string cron) where T : IJob
        {
            try
            {
                IJobDetail job = JobBuilder.Create<T>()
                       .WithIdentity(nameJob, nameGroup)
                       .Build();

                ITrigger trig = TriggerBuilder.Create()
                         .WithIdentity(nameTrigger, nameGroupnameTrigger)
                         .WithCronSchedule(cron)
                         .ForJob(job)
                         .StartNow()
                         .Build();
                scheduler.ScheduleJob(job, trig);
                return true;
            }
            catch
            {
                return false;
            }
        }
      
        #endregion
        #region public method
        public static string path { get; set; }
        public bool onStart()
        {
            try
            {
                scheduler.Start();
                LogsSingleton.WriteText("Job start successful.","Quarzt");
                //QuarztSingleton.Instance.createJob<onValidateHolidayAsync>("ValdateHolidateTrig", "sys", "ValdateHolidateJob", "sys", "00:00:60");
                return true;
            }
            catch (Exception ex)
            {
                LogsSingleton.WriteText("Job failed. Error detail: \n " + ex.Message, "Quarzt");              
                return false;
            }
        }
        public bool createJob<T>(string nameJob, string nameGroup, string nameTrigger, string nameGroupnameTrigger, string time) where T : IJob
        {
            try
            {
                int hours = int.Parse(time.Substring(0, 2).ToString().TrimStart());
                int minutes = int.Parse(time.Substring(3, 2).ToString().TrimStart());
                int second = int.Parse(time.Substring(6, 2).ToString().TrimStart());

                scheduler.ScheduleJob(addJob<T>(nameJob, nameGroup), addTrigger(nameTrigger, nameGroupnameTrigger, second));
                scheduler.Start();
                LogsSingleton.WriteText("Create job successful.", "Quarzt");
              
                return true;
            }
            catch (Exception ex)
            {
                LogsSingleton.WriteText("Create job failed. Error detail: \n " + ex.Message, "Quarzt");
            
                return false;
            }
        }
        public bool createJobbyCron<T>(string nameTrigger, string nameGroupnameTrigger, string nameJob, string nameGroup, string cron) where T : IJob
        {
            try
            {
                addCron<T>(nameJob, nameGroup, nameTrigger, nameGroupnameTrigger, cron);
                LogsSingleton.WriteText("Create cron successful", "Quarzt");    
                return true;
            }
            catch (Exception ex)
            {

                LogsSingleton.WriteText("Create cron failed. Error detail: \n " + ex.Message, "Quarzt");          
                return false;
            }
        }

        public static void GetAllJobs()
        {
            var allTriggerKeys = scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            foreach (var triggerKey in allTriggerKeys.Result)
            {
                var triggerdetails = scheduler.GetTrigger(triggerKey);
                var Jobdetails = scheduler.GetJobDetail(triggerdetails.Result.JobKey);
                Console.WriteLine(triggerdetails.Result.Key.Name + " Job key -" + triggerdetails.Result.JobKey.Name + " " + triggerdetails.Result.CalendarName);
            }
        }

        public bool onPause()
        {
            try
            {
                scheduler.Shutdown();
                LogsSingleton.WriteText("Job shutdown successful.", "Quarzt");           
                return true;
            }
            catch (Exception ex)
            {
                LogsSingleton.WriteText("Job shutdown failed. Error detail: \n" + ex.Message, "Quarzt");             
                return false;
            }
        }


        #endregion

    }
    public class onValidateHolidayAsync : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            HolidayCalendar calendar = new HolidayCalendar();

            DayOfWeek toDay = DateTime.Now.DayOfWeek;
            if ((toDay == DayOfWeek.Saturday) || (toDay == DayOfWeek.Sunday))
            {
                calendar.AddExcludedDate(DateTime.Now);
                QuarztSingleton.scheduler.AddCalendar("Holiday", calendar, true, true);

                ITrigger trigger = QuarztSingleton.scheduler.GetTrigger(new TriggerKey("NamHTtrigger", "NamHT")).GetAwaiter().GetResult();
                TriggerBuilder tb = trigger.GetTriggerBuilder();

                ITrigger UpdateTrigger = tb.ModifiedByCalendar("Holiday").Build();

                QuarztSingleton.scheduler.RescheduleJob(new TriggerKey("NamHTtrigger", "NamHT"), UpdateTrigger);

                LogsSingleton.WriteText("Hôm nay, ngày nghỉ Job đi nhậu với bạn đây.", "Quarzt");

            }
            return Task.FromResult("done");
        }
    }
}
