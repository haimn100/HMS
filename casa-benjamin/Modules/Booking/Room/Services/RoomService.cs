using casa_benjamin.Modules.Booking.Lodging.Enums;
using casa_benjamin.Modules.Shared.Repositories;
using System.Collections.Generic;

namespace casa_benjamin.Modules.Booking.Room.Services
{
    public class RoomService
    {
        private GenericRepository repository;
        private const string ROOMS_TABLE = "room";
        private const string ROOMBEDS_TABLE = "room_bed";

        public RoomService(string dbConnectionString)
        {
            repository = new GenericRepository();
        }

        public List<Entities.Room> AllRooms()
        {
            return repository.GetAll<Entities.Room>();
        }

        public Entities.Room FindOne(int id)
        {
            return repository.GetOne<Room.Entities.Room>($"select * from {ROOMS_TABLE} where id = {id}");
        }

        public List<Entities.RoomBed> FindBedsByRoom(int roomId)
        {
            return repository.Get<Entities.RoomBed>($"select * from {ROOMBEDS_TABLE} where room_id = {roomId}");
        }

    }
}