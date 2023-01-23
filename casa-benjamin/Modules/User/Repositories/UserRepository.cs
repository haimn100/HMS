using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper.Contrib.Extensions;
using System.Transactions;
using System;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.Shared.Enums;

namespace casa_benjamin.Modules.User.Repositories
{
    public class UserRepository
    {
        GenericRepository genericRepository = new GenericRepository();

        public Modules.User.Entities.User GetGhostUser()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where is_hidden = 1").FirstOrDefault();
            }
        }

        public int CreateGhostUser(int ghostBedId)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Execute(@"insert into user (`passport`,`cidate`,`name`,`bed_id`,`is_hidden`) values(1234,@date,'system',@bed,1)",new { date = DateTime.Now,bed= ghostBedId });
            }
        }

        public Bed GetGhostBed()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Bed>("select * from bed where is_hidden = 1").FirstOrDefault();
            }
        }

        public int CreateGhostBed()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Execute("insert into bed (bed_type_id,is_hidden) values(1,1)");
            }
        }

        public Modules.User.Entities.User GetUser(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where id = @id", new { id }).FirstOrDefault();
            }
        }

        public Modules.User.Entities.User  GetUser(string email)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where email = @email", new { email }).FirstOrDefault();
            }
        }

        public Staff.Entities.Staff GetStaff(string email)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.Staff.Entities.Staff>("select * from staff where email = @email", new { email }).FirstOrDefault();
            }
        }

        public List<Modules.Staff.Entities.Staff> GetStaff()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.Staff.Entities.Staff>("select * from staff where is_working = 1").ToList();
            }
        }

        public Staff.Entities.Staff GetStaff(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.Staff.Entities.Staff>("select * from staff where id=@id",new { id }).FirstOrDefault();
            }
        }
       

        public List<Modules.User.Entities.User> GetAllCheckedInUsers()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where is_checked_out = 0 and (is_hidden is null or is_hidden = 0)").ToList();
            }
        }

       
        public List<Modules.User.Entities.User> GetAllResidents()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from resident").ToList();
            }
        }

        public List<UserBed> GetUserBeds(int userId)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<UserBed> ("select * from user_bed where user_id = " + userId).ToList();
            }
        }

        public long InsertGuest(User.Entities.User  user,int bedPrice, string comment)
        {
            int userId = -1;
            using (var transactionScope = new TransactionScope())
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
                {
                    con.Open();
                    userId = (int)con.Insert(user);

                    con.Insert(new UserBed
                    {
                        bed_id = user.bed_id,
                        price = bedPrice,
                        user_id = userId,
                        start_date = user.cidate
                    });
                }
                transactionScope.Complete();
            }

            return userId;
        }

        public bool MoveGuest(int guestId, int destBed, int bedPrice,string comment)
        {
            using (var transactionScope = new TransactionScope())
            {

                User.Entities.User user  = GetUser(guestId);
                List<UserBed> userBeds = GetUserBeds(guestId);
                UserBed lastBed = userBeds.Last();

                //if try to move in the same day we will update the records and not insert new one
                if(lastBed.start_date.Day == DateTime.Now.Day)
                {
                    lastBed.bed_id = destBed;
                    lastBed.price = bedPrice;
                    genericRepository.Update(lastBed);                
                }
                else
                {
                    lastBed.end_date = DateTime.Now;
                    genericRepository.Update(lastBed);                
                    genericRepository.Insert(new UserBed
                    {
                        start_date = lastBed.end_date.Value,
                        comment = comment,
                        bed_id = destBed,
                        price = bedPrice,
                        user_id = user.id
                    });
                }              

                user.bed_id = destBed;
                UpdateUser(user);

                transactionScope.Complete();
            }

            return true;
        }

        public bool UpdateUser(User.Entities.User  user)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Update(user);
            }
        }

        public bool UpdateBed(Bed bed)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Update(bed);
            }
        }
        

        public User.Entities.User  GetUserByBed(int bedId)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where is_checked_out = 0 and bed_id = @bedId", new { bedId }).FirstOrDefault();
            }
        }

        public User.Entities.User  GetUserByPassport(string passport)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Modules.User.Entities.User>("select * from user  where passport = @passport", new { passport }).FirstOrDefault();
            }
        }

        public Shift GetLastShift()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Shift>("select * from shift_end order by id desc limit 1").FirstOrDefault();
            }
        }

        public List<Order> GetShiftCashAndCreditOrders(DateTime endOfShiftDate, DateTime? lastShiftDate)
        {            
            DateTime _lastShiftDate = !lastShiftDate.HasValue ? DateTime.Now.AddYears(-1) : lastShiftDate.Value;
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Order>("select * from menu_order where pay_type_id != 1 and order_date <= @endOfShiftDate and order_date > @_lastShiftDate", new { ptype = PayType.Cash, endOfShiftDate, _lastShiftDate }).ToList();
            }
        }

        public long InsertStaff(Staff.Entities.Staff member)
        {
            return genericRepository.Insert(member);
        }

        public bool DeleteStaff(int id)
        {
            return genericRepository.Delete(new Staff.Entities.Staff { id = id });
        }
    }
}