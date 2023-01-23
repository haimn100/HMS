using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.Booking.Room.Enums;
using casa_benjamin.Modules.Booking.Room.Services;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.User.Services;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    public class RoomController : Controller
    {

        private readonly RoomService roomService = new RoomService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
        
        // GET: Room
        public bool MarkClean(int roomId, bool isCleanRequired)
        {
            Room room = CacheManager.Instance.Rooms.First(x => x.room.id == roomId).room;
            room.is_clean_required = isCleanRequired;
            UserManager.Instance.UpdateRoom(room);
            CacheManager.Instance.RefreshRooms();
            return true;
        }

        public void AddNote(int roomid, string note)
        {
            Room room = UserManager.Instance.GetRoom(roomid);
            room.note = note;
            UserManager.Instance.UpdateRoom(room);
            CacheManager.Instance.RefreshRooms();

        }

        public void DeleteNote(int roomid)
        {
            Room room = UserManager.Instance.GetRoom(roomid);
            room.note = null;
            UserManager.Instance.UpdateRoom(room);
            CacheManager.Instance.RefreshRooms();

        }

        public ActionResult GetRooms()
        {
            return new JsonResult { Data = roomService.AllRooms(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetRoomsSelectList()
        {
            var data = roomService.AllRooms()
                .Select(x => new { id = x.id, name = $"{x.id} ({ x.room_type_id})"});
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetRoomsBeds()
        {
            return new JsonResult { Data = UserManager.Instance.GetRoomBeds(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}