using casa_benjamin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using casa_benjamin.Helpers; 
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Services;

using casa_benjamin.Modules.Booking.Room.Enums;
using System.Transactions;
using casa_benjamin.Modules.Shared.Values;
using casa_benjamin.Modules.Booking.Room.Services;
using System.Configuration;
using casa_benjamin.Modules.User.Services;

namespace casa_benjamin.Managers
{
    public class ReservationManager
    {
        private static volatile ReservationManager instance;
        private static object syncRoot = new Object();
        private const string RESERVATION_TABLE = "reservation";

        private GenericRepository genericRepository;
        private RoomService roomService;

        private ReservationManager()
        {
            genericRepository = new GenericRepository();
            roomService = new RoomService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
        }

        public static ReservationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ReservationManager();
                    }
                }

                return instance;
            }
        }

        public List<Reservation> GetReservations(DateTime date, int room, int? excludeRes = null)
        {
            string q = "select * from (select * from reservation where res_date = '" + date.ToMySqlDateString() + "' and room_id =" + room + ") t1 group by res_id";

            if (excludeRes.HasValue)
            {
                q = "select * from (select * from reservation where res_date = '" + date.ToMySqlDateString() + "' and room_id =" + room + " and res_id != " + excludeRes.Value + ") t1 group by res_id";
            }

            return genericRepository.Get<Reservation>(q).ToList();
        }

        public List<Reservation> GetNonCheckedOutReservations(DateTime date, int room, int? excludeRes = null)
        {
           
            string q = "select * from (select * from reservation where res_date = '" + date.ToMySqlDateString() + "' and room_id =" + room + " and status != 3) t1 group by res_id";
            if (excludeRes.HasValue)
            {
                q = "select * from (select * from reservation where res_date = '" + date.ToMySqlDateString() + "' and room_id =" + room + "  and status != 3 and res_id != " + excludeRes.Value + ") t1 group by res_id";
            }
            return genericRepository.Get<Reservation>(q).ToList();
        }

        public List<Reservation> GetReservationsList()
        {
            return genericRepository.Get<Reservation>("select * from (select * from reservation order by id asc) t1 group by res_id order by res_date asc limit 100").ToList();
        }

        public List<Reservation> GetReservationsList(DateTime from, DateTime to)
        {
            string query = $"select * from reservation where res_date >= '{from.ToMySqlDateString()}' and res_date < '{to.ToMySqlDateString()}' order by res_date";
            return genericRepository.Get<Reservation>(query).ToList();
        }

        public List<Reservation> GetReservationsGroupedList(DateTime from, DateTime to, string name = null)
        {
            string nameQuery = !string.IsNullOrEmpty(name) ? $"and res_name like '%{name}%'" : ""; ;
            string query = $"select * from reservation where res_id in (select distinct(res_id) from reservation where res_date >= '{from.ToMySqlDateString()}' and res_date < '{to.ToMySqlDateString()}' {nameQuery}) " +
                $"group by res_id,room_id order by res_date";

            return genericRepository.Get<Reservation>(query).ToList();
        }

        public PagedTableResponse<Reservation> GetReservationsTable(PagedTableRequest req)
        {
            req.table = RESERVATION_TABLE;
            req.dateField = "res_date";
            req.groupBy = $"res_id,room_id";
            if (!string.IsNullOrEmpty(req.search)) req.sortBy = "res_name";

            return genericRepository.GetTable<Reservation>(req);                     
        }
     

        public Reservation GetReservation(int reservationId)
        {
            return genericRepository.Get<Reservation>("select * from reservation where res_id=" + reservationId + " order by res_date asc").FirstOrDefault();
        }

        public List<Reservation> GetReservationAllDates(int reservationId)
        {
            return genericRepository.Get<Reservation>("select * from reservation where res_id=" + reservationId + " order by res_date asc");
        }

        public List<Reservation> GetReservationsByRoom(int roomId)
        {
            return genericRepository.Get<Reservation>("select * from reservation where room_id=" + roomId).ToList();
        }

        public List<Reservation> GetReservationsByRoom(int roomId, DateTime day)
        {
            return genericRepository.Get<Reservation>("select * from reservation where room_id=" + roomId + " and res_date = '" + day.ToMySqlDateString() + " '").ToList();
        }
        public List<Reservation> GetNonCheckoutReservationsByRoom(int roomId, DateTime day)
        {
            return genericRepository.Get<Reservation>("select * from reservation where room_id=" + roomId + " and res_date = '" + day.ToMySqlDateString() + " ' and status != 3").ToList();
        }

        public Reservation FindOne(int id, int roomId)
        {
            return genericRepository.Get<Reservation>($"select * from reservation where res_id={id} and room_id = {roomId} order by res_date asc").FirstOrDefault();
        }

        public Reservation FindOne(int id)
        {
            return genericRepository.Get<Reservation>($"select * from reservation where res_id={id}").FirstOrDefault();
        }

        public List<Reservation> FindMany(string ids, int roomId)
        {
            return genericRepository.Get<Reservation>($"select * from reservation where res_id in ({ids}) and room_id = {roomId}").ToList();
        }

        /// <summary>
        /// return error or null
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="reservationId"></param>
        /// <returns></returns>
        public string AddReservation(Reservation reservation)
        {
            if (reservation.res_id <= 0) { throw new Exception("must have res id"); }

            reservation.nights = CalcNights(reservation.res_date, reservation.res_date_end);
            if (reservation.nights < 1) { return "?alert=problem calculating nights"; }

            UIRoom room = CacheManager.Instance.Rooms.FirstOrDefault(x => x.room.id == reservation.room_id);
            if (room == null) { throw new Exception("Room does not exist"); }

            string checkAvailabilityResult = CheckReservationAvailability(room.room.id, reservation.res_date, reservation.res_date_end, reservation.number_of_people);
            if (checkAvailabilityResult != "ok")
            {
                throw new InvalidOperationException(checkAvailabilityResult);
            }

            reservation.room_type = room.room.room_type_id;
            DateTime startDate = reservation.res_date.AddDays(-1);
            for (int i = 0; i < reservation.nights; i++)
            {
                startDate = startDate.AddDays(1);
                var newRes = reservation.ShallowCopy();
                newRes.res_date = startDate;
                genericRepository.Insert(newRes);
            }

            return null;
        }

        public void DeleteReservation(int rid)
        {
            var users = UserManager.Instance.GetActiveUsers();
            var usersWithReservation = users.Where(x => x.res_id.HasValue && x.res_id.Value == rid).ToList();
            if(usersWithReservation.Count > 0)
            {
                throw new Exception("Please check out all users of this reservation");
            }
            
            genericRepository.ExecuteScalar("delete from reservation where res_id = " + rid);
        }

        public void UpdateReservation(Reservation res)
        {
            int resId = res.res_id;
            
            var resAllDatesDB = genericRepository.Get<Reservation>("select * from reservation where res_id= " + res.res_id).ToList();         
            var firstRes = resAllDatesDB.OrderBy(x => x.res_date).First();

            bool startDateChanged = res.res_date != firstRes.res_date;
            bool endDateChanged = res.res_date_end != firstRes.res_date_end;

            if (res.room_id != firstRes.room_id || res.number_of_people != firstRes.number_of_people)
            {
                string checkResAvailResult = CheckReservationAvailability(res.room_id, res.res_date, res.res_date_end, res.number_of_people, res.res_id);
                if (checkResAvailResult != "ok") { throw new InvalidOperationException(checkResAvailResult); }
            }

            if (startDateChanged || endDateChanged)
            {
                resId = ChangeReservationDates(res, res.res_date, res.res_date_end);
            }

            var room = roomService.FindOne(res.room_id);

            genericRepository.ExecuteScalar($"update reservation set " +
                $"allow_mix_dorm = @allow_mix_dorm ," +
                $" comment = @comment ," +
                $" res_name = @res_name ," +
                $" sex = @sex ," +
                $" res_email = @res_email ," +
                $" number_of_people = @number_of_people ," +
                $" status = @status ," +
                $" room_id = @room_id," +
                $" room_type = @room_type," +
                $" from_channel_manager = @from_channel_manager" +
                $" where res_id = {resId}"
                , new
                {
                    allow_mix_dorm = res.allow_mix_dorm,
                    comment = res.comment,
                    res_name = res.res_name,
                    sex = res.sex,
                    res_email = res.res_email,
                    number_of_people = res.number_of_people,
                    status = res.status,
                    room_id = res.room_id,
                    room_type = room == null ? res.room_type : room.room_type_id,
                    from_channel_manager = res.from_channel_manager
                });
                //resAllDatesDB.ForEach((dbRes) =>
                //{
                //    var resCopy = dbRes.ShallowCopy();
                //    dbRes.allow_mix_dorm = res.allow_mix_dorm;
                //    dbRes.comment = res.comment;
                //    dbRes.res_name = res.res_name;
                //    dbRes.sex = res.sex;
                //    dbRes.res_email = res.res_email;
                //    dbRes.number_of_people = res.number_of_people;
                //    dbRes.status = res.status;
                //    dbRes.room_id = res.room_id;
                //    genericRepository.Update(dbRes);
                //});            
        }

        public void UpdateReservationRoom(int resId, int room, int roomType)
        {
            genericRepository.ExecuteScalar("update reservation set room_id = " + room + ", room_type = " + roomType + " where res_id = " + resId);
        }

        public void UpdateReservationEndDate(int resId, DateTime endDate, int nights)
        {
            genericRepository.ExecuteScalar("update reservation set res_date_end = '" + endDate.ToMySqlDateString() + "', nights = " + nights + " where res_id = " + resId);
        }

        public void UpdateReservationDate(int resId, DateTime startDate, DateTime endDate)
        {
            //by ascending order
            var originalResDates = GetReservationAllDates(resId);
            var originalResEndDate = originalResDates.First().res_date_end.AddDays(-1);
            var reqEndDate = endDate.AddDays(-1);

            /* bool = true(do nothing) / false (delete it) */
            var originialDatesDic = new Dictionary<DateTime, bool>();
            Array.ForEach(originalResDates.ToArray(), (x) => originialDatesDic.Add(x.res_date, false));
            
            //bool = true(add it) / false (do nothing)
            var requestedDaysDic = new Dictionary<DateTime, bool>();
            //populate req dates list with default true=add it
            Array.ForEach(DateRangeToList(startDate,reqEndDate).ToArray(), (x) => requestedDaysDic.Add(x, true));

            foreach (var dateBool in originialDatesDic.ToList())
            {
                //if the date from the existing reservation exists in the requested reservation dates
                originialDatesDic[dateBool.Key] = IsBewteenTwoDates(dateBool.Key, startDate, reqEndDate);
            }

            foreach (var dateBool in requestedDaysDic.ToList())
            {
                //if the date from the requested reservation dates exist in the existing reservation
                requestedDaysDic[dateBool.Key] = !IsBewteenTwoDates(
                    dateBool.Key,
                    originalResDates.First().res_date,
                    originalResEndDate
                    );
            }

            using (var transactionScope = new TransactionScope())
            {
                //if x.Value = true,  add the the date
                foreach (var item in requestedDaysDic.Where(x=>x.Value))
                {
                    var newResDate = originalResDates.First().ShallowCopy();
                    newResDate.res_date = item.Key;
                    newResDate.res_date_end = endDate;
                    if(genericRepository.Get<Reservation>($"select * from {RESERVATION_TABLE} where res_id = {resId} and res_date = '{item.Key.ToMySqlDateString()}'").Count == 0)
                    {
                        genericRepository.Insert(newResDate);

                    }
                }

                //if x.Value = false, delete it
                foreach (var item in originialDatesDic.Where(x => !x.Value))
                {
                    genericRepository.ExecuteScalar($"delete from {RESERVATION_TABLE} where id = {originalResDates.First(x => x.res_date == item.Key).id}");
                }

                //update all the reservation records end date
                genericRepository.ExecuteScalar($"update {RESERVATION_TABLE} set res_date_end = '{endDate.ToMySqlDateString()}' where res_id = {resId}");

                try
                {
                    transactionScope.Complete();
                }
                catch { transactionScope.Dispose(); }
            }
        }

        public void UpdateReservationStatus(int resId, ReserVationStatus status)
        {
            genericRepository.ExecuteScalar("update reservation set status = " + (int)status + " where res_id = " + resId);
        }

        public List<Reservation> GetReservationCalendar(DateTime from, DateTime to)
        {
            string innerQuery = string.Format("select * from reservation where status != 3 and (res_date between '{0}' and '{1}' or res_date_end between '{0}' and '{1}') order by res_date asc", from.ToMySqlDateString(), to.ToMySqlDateString());
            string sql = "select * from (" + innerQuery + ") t1 group by res_id";

            return genericRepository.Get<Reservation>(sql).ToList();
        }

        public RoomBed GetBedByReservation(int reservationId)
        {
            return genericRepository.Get<RoomBed>("select * from room_bed where reservation_id = " + reservationId).FirstOrDefault();
        }

        public string CheckReservationAvailability(int roomNumber, DateTime from, DateTime to, int numOfPeople, int? excludeRes = null)
        {
            var nights = Convert.ToInt32((to - from).TotalDays);
            if (nights == 0) nights = 1;
            UIRoom room = CacheManager.Instance.GetUIRoom(roomNumber);
            var numOfBedsInRoom = room.beds.Count;

            var date = new DateTime(from.Year, from.Month, from.Day);

            for (int i = 0; i < nights; i++)
            {
                int bedsTaken = 0;
                List<Reservation> existingReservations = Instance.GetNonCheckedOutReservations(date, room.room.id, excludeRes);
                bedsTaken = existingReservations.Sum(x => x.number_of_people);

                if (room.room.room_type_id == RoomType.Private && bedsTaken > 0)
                {
                    return string.Format("?alert=Room {0} is full in {1}", room.room.id, date.ToShortUIDateString());
                }

                if (bedsTaken == numOfBedsInRoom)
                {
                    return string.Format("?alert=Room {0} is full in {1}", room.room.id, date.ToShortUIDateString());
                }

                if ((bedsTaken + numOfPeople) > numOfBedsInRoom)
                {
                    return string.Format("?alert=Room {0} does not have enough beds in {1}", room.room.id, date.ToShortUIDateString());
                }
                date = date.AddDays(1);
            }

            return "ok";
        }

        private int GetRoomMaxOccupancy (int roomId)
        {
            var beds = roomService.FindBedsByRoom(roomId);
            return beds.Count;
        }


        public void ExtendReservation(Reservation res, DateTime endDate)
        {
            res.res_date_end = endDate;
            res.nights = Instance.CalcNights(res.res_date, res.res_date_end);
            UpdateReservationEndDate(res.res_id, endDate, res.nights);

            DateTime resDate = res.res_date_end;
            while (resDate < endDate)
            {
                Reservation resToAdd = res.ShallowCopy();
                resToAdd.res_date = resDate;
                resToAdd.res_date_end = endDate;
                genericRepository.Insert(resToAdd);
                resDate = resDate.AddDays(1);
            }

        }

        private int GetNumOfBeds(casa_benjamin.Models.UIRoom room)
        {
            double numOfRoomBeds = 0;
            foreach (var item in room.beds)
            {
                if (room.room.room_type_id == RoomType.Private)
                {
                    numOfRoomBeds += 1;
                }
                else
                {
                    numOfRoomBeds += item.bed_type_id == BedType.Double ? 0.5 : 1;

                }
            }

            return (int)numOfRoomBeds;
        }

        private int ChangeReservationDates(Reservation reservation, DateTime from, DateTime to)
        {
            string result = CheckReservationAvailability(reservation.room_id, from, to, reservation.number_of_people, reservation.res_id);
            if (result != "ok") { throw new InvalidOperationException(result); }
            UpdateReservationDate(reservation.res_id, from, to);               
            return reservation.res_id;           
        }

        public int CalcNights(DateTime from, DateTime to)
        {
            return Convert.ToInt32((to.Date - from.Date).TotalDays);
        }


        public bool IsBewteenTwoDates(DateTime dt, DateTime start, DateTime end, bool startInclusive = true, bool endInclusive = true)
        {
            bool startMatch = startInclusive ? dt >= start : dt > start;
            bool endMatch = endInclusive ? dt <= end : dt < end;
            return startMatch && endMatch;
        }

        public List<DateTime> DateRangeToList(DateTime start, DateTime end)
        {
            if (start > end) throw new Exception("Invalid Date");

            List<DateTime> result = new List<DateTime>();
            DateTime d = start;
            while(d <= end)
            {
                result.Add(d);
                d = d.AddDays(1);
            }

            return result;
        }
    }
}