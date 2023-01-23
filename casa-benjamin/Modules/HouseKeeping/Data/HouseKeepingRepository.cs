using casa_benjamin.Helpers;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.HouseKeeping.Data.Models;
using casa_benjamin.Modules.HouseKeeping.Models;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Shared.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace casa_benjamin.Modules.HouseKeeping.Data
{
    public class HouseKeepingRepository : IHouseKeeperRepository
    {
        IGenericRepository GenericRepository { get; set; }
        public HouseKeepingRepository(IGenericRepository genericRepository)
        {
            GenericRepository = genericRepository;
        }

        public List<HouseKeeper> AllHouseKeepers(bool onlyActive = true)
        {
            if (onlyActive)
            {
                return GenericRepository.Get<HouseKeeper>("select * from house_keeper where is_active=1").ToList();
            }
            else
            {
                return GenericRepository.Get<HouseKeeper>("select * from house_keeper").ToList();
            }
        }

        public PagedTableResponse<HouseKeepingTracking> HouseKeepingHistoryTable(PagedTableRequest req)
        {
            return GenericRepository.GetTable<HouseKeepingTracking>(req);
        }

        public long RequireCleaning(RequireCleaningRequest req)
        {

            var id = GenericRepository.Insert(new HouseKeepingTracking
            {
                room_number = req.room_number,
                house_keeper_id = req.house_keeper_id,
                house_keeper_name = req.house_keeper_name,
                assigned_date = DateTime.Now
            });

            Room room = GenericRepository.Get<Room>("select * from room where room_number = " + req.room_number).First();
            room.is_clean_required = true;
            room.is_cleaning_inspection_required = false;
            room.is_cleaning_inspection_date = null;
            room.assigned_house_keeper_name = req.house_keeper_name;
            room.assigned_house_keeper_date = DateTime.Now;
            room.assigned_house_keeper_id = req.house_keeper_id;
            room.house_keeping_tracking_id = (int)id;
            GenericRepository.Update(room);
            CacheManager.Instance.RefreshRooms();

            return id;
        }

        public void FinishCleaning(FinishCleaningRequest req)
        {
            Room room = GenericRepository.Get<Room>("select * from room where room_number = " + req.room_number).First();
            var hkTracking = GetHouseKeepingTracking(room.house_keeping_tracking_id.Value);

            hkTracking.finish_date = DateTime.Now;
            hkTracking.num_of_beds_cleaned = req.num_of_beds;
            hkTracking.comment = req.comment;

            GenericRepository.Update(hkTracking);

            room.assigned_house_keeper_name = null;
            room.assigned_house_keeper_id = null;
            room.house_keeping_tracking_id = null;
            room.is_cleaning_inspection_date = null;
            room.assigned_house_keeper_date = null;
            room.is_cleaning_inspection_required = false;
            room.is_clean_required = false;
            room.last_cleaned = DateTime.Now;
            GenericRepository.Update(room);
            CacheManager.Instance.RefreshRooms();
        }

        public HouseKeepingTracking GetHouseKeepingTracking(int id)
        {
            return GenericRepository.Get<HouseKeepingTracking>("select * from house_keeping_tracking where id = " + id).First();
        }

        public List<HouseKeepingTracking> GetHouseKeepingTrackingReport(DateTime from, DateTime to, int? keeperId, int? roomId)
        {
            string q = $@"SELECT * FROM house_keeping_tracking
                            where assigned_date between '{from.ToMySqlDateTimeString()}' and '{to.ToMySqlDateTimeString()}'";

            if (keeperId.HasValue)
            {
                q += $" and house_keeper_id = {keeperId.Value}";
            }

            if (roomId.HasValue)
            {
                q += $" and room_number = {roomId.Value}";
            }

            return GenericRepository.Get<HouseKeepingTracking>(q).ToList();
        }

        public void DeleteHK(int id)
        {
            GenericRepository.Delete(new HouseKeeper { id = id });
        }

        public void AddHK(HouseKeeper hk)
        {
            GenericRepository.Insert(hk);
        }
    }

    public interface IHouseKeeperRepository
    {
        long RequireCleaning(RequireCleaningRequest req);
        void FinishCleaning(FinishCleaningRequest req);
        void DeleteHK(int id);
        void AddHK(HouseKeeper hk);

        List<HouseKeeper> AllHouseKeepers(bool onlyActive = true);
        PagedTableResponse<HouseKeepingTracking> HouseKeepingHistoryTable(PagedTableRequest req);
        HouseKeepingTracking GetHouseKeepingTracking(int id);
        List<HouseKeepingTracking> GetHouseKeepingTrackingReport(DateTime from, DateTime to, int? keeperId,int? roomId);
    }

}