using casa_benjamin.Managers;
using casa_benjamin.Models;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.User.Entities;
using NLog;
using System;
using System.Diagnostics;
using System.Timers;

namespace casa_benjamin.Modules.Shared.Services
{
    public class SystemTaskManager
    {
        private static volatile SystemTaskManager instance;
        private static object syncRoot = new Object();
        private bool tasksStarted = false;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static SystemTaskManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SystemTaskManager();
                    }
                }

                return instance;
            }
        }

        public void StartTasks()
        {
            if (tasksStarted)
                return;

            tasksStarted = true;

            StockAlertTask();
            GuestCountTask();
        }

        private void StockAlertTask()
        {
            //Timer timer = new Timer(1000 * 60 * 10);
            //try
            //{
            //    timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            //    {
            //        timer.Stop()             

            //        timer.Start();
            //    };
            //    timer.Start();
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        timer.Stop();
            //        timer.Enabled = false;
            //    }
            //    catch (Exception e) {
            //        logger.Error(e);
            //    }
            //    Trace.TraceError(ex.ToString());
            //}
        }

        private void GuestCountTask()
        {
            Timer timer = new Timer(1000 * 60 * 60 *3);
            try
            {
                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    timer.Stop();

                    _GuestCountTask();

                    timer.Start();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    timer.Stop();
                    timer.Enabled = false;
                }
                catch (Exception e)
                {
                    logger.Error(ex);
                }
                Trace.TraceError(ex.ToString());
            }

            _GuestCountTask();
        }

        private void UserNightsTask()
        {
            Timer timer = new Timer(1000 * 60 * 60 * 1);
            try
            {
                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                {
                    timer.Stop();

                    _UserNightTask();

                    timer.Start();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    timer.Stop();
                    timer.Enabled = false;
                }
                catch (Exception e)
                {
                    logger.Error(ex);
                }
                Trace.TraceError(ex.ToString());
            }

            _UserNightTask();
        }

        private void _GuestCountTask()
        {
            DateTime now = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            GuestCount lastGuestCount = ReportsManager.Instance.GetLatestGuestCount();
            if (lastGuestCount != null)
            {
                if(lastGuestCount.guestcount_date <= now)
                {
                    DateTime from = lastGuestCount.guestcount_date;
                    while (from <= now)
                    {
                        long guestCount = ReportsManager.Instance.CalcGuestsCount(from);
                        ReportsManager.Instance.InsertGuestCount(new GuestCount
                        {
                            guestcount_date = from,
                            count = guestCount
                        });
                        from = from.AddDays(1);
                    }
                }
            }
            else
            {
                User.Entities.User firstUser = ReportsManager.Instance.GetFirstUser();
                if(firstUser != null)
                {
                    DateTime from = firstUser.cidate;

                    while (from <= now)
                    {
                        long guestCount = ReportsManager.Instance.CalcGuestsCount(from);
                        ReportsManager.Instance.InsertGuestCount(new GuestCount
                        {
                            guestcount_date = from,
                            count = guestCount
                        });
                        from = from.AddDays(1);
                    }
                }                
            }
        }

        private void _UserNightTask()
        {
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            try
            {
                UserNightTaskLog lastLog = NightsManager.Instance.GetLastNightTaskDate();
                if (lastLog != null)
                {
                    DateTime logDate = new DateTime(lastLog.task_date.Year, lastLog.task_date.Month, lastLog.task_date.Day);

                    if (logDate < now)
                    {
                        NightsManager.Instance.AddNightsTask();
                        NightsManager.Instance.AddNightsTaskComplete(now, null);
                    }
                }
                else
                {
                    NightsManager.Instance.AddNightsTask();
                    NightsManager.Instance.AddNightsTaskComplete(now, null);

                }
            }
            catch(Exception ex)
            {
                NightsManager.Instance.AddNightsTaskComplete(now, ex.ToString());

            }
        }
    }
}