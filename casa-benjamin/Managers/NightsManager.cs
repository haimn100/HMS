using casa_benjamin.Data;
using casa_benjamin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using casa_benjamin.Extensions;
using casa_benjamin.Helpers;
using casa_benjamin.Modules.User;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.User.Services;

namespace casa_benjamin.Managers
{
    public class NightsManager
    {
        private static volatile NightsManager instance;
        private static object syncRoot = new Object();

        private GenericRepository genericRepository;


        public static NightsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new NightsManager();
                    }
                }

                return instance;
            }
        }

        private NightsManager()
        {
            genericRepository = new GenericRepository();
        }

        public void AddNight(UserNight userNight)
        {
            genericRepository.Insert(userNight);
        }

        public void DeleteNight(int userId, DateTime date)
        {
            genericRepository.ExecuteScalar($"delete from user_night where user_id = {userId} and night_date = '{date.ToMySqlDateString()}'");
        }

        public void ChangeLastNightPrice(string staff_name, int userId,int price)
        {
            var user = UserManager.Instance.GetUser(userId);
            var nights = GetUserNights(userId);
            var lastnight = nights.Last();

            lastnight.price = price;
            genericRepository.Update(lastnight);

            genericRepository.Insert<UserNightLog>(new UserNightLog
            {
                action = "Changed Price",
                current_date = DateTime.Now,
                staff_name = staff_name,
                user_id = user.id,
                user_name = user.name,
                action_val = price.ToString()
            });

        }


        public bool IsUserExistInTable(int userId)
        {
            var nights = GetUserNights(userId);
            return (nights.Count != 0);
        }


        List<UserNight> GetUserNights(int userId)
        {
            return genericRepository.Get<UserNight>($"select * from user_night where user_id = {userId} order by night_date asc").ToList();
        }


        public void AddNightsTask()
        {
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var users = UserManager.Instance.GetActiveUsers();


            foreach (var user in users)
            {
                List<UserNight> nights = GetUserNights(user.id);
                var lastNight = nights.Last();
                var lastNightDate = new DateTime(lastNight.night_date.Year, lastNight.night_date.Month, lastNight.night_date.Day);


                if (lastNightDate < now)
                {
                    while (lastNightDate < now)
                    {
                        lastNightDate = lastNightDate.AddDays(1);
                        AddNight(new UserNight
                        {
                            night_date = lastNightDate,
                            price = lastNight.price,
                            user_id = user.id
                        });
                    }

                    //genericRepository.ExecuteScalar($"update user set ");
                }
            }
        }

        public void AddNightsTaskComplete(DateTime date,string error)
        {
            genericRepository.Insert(new UserNightTaskLog
            {
                error = string.IsNullOrEmpty(error) ? "": error.Substring(1, 1000),
                success = string.IsNullOrEmpty(error) ? 1 : 0,
                task_date = date
            });
        }
        
        public UserNightTaskLog GetLastNightTaskDate()
        {
            return genericRepository.Get<UserNightTaskLog>("select * from user_night_task_log order by id desc limit 1").FirstOrDefault();
        }

    }
}