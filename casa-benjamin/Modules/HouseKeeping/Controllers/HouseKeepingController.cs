using casa_benjamin.ActionFilters;
using casa_benjamin.Modules.HouseKeeping.Data;
using casa_benjamin.Modules.HouseKeeping.Data.Models;
using casa_benjamin.Modules.HouseKeeping.Models;
using System;
using System.Web.Mvc;

namespace casa_benjamin.Modules.HouseKeeping.Controllers
{
    public class HouseKeepingController : Controller
    {
        IHouseKeeperRepository HouseKeeperRepository { get; set; }

        public HouseKeepingController(IHouseKeeperRepository houseKeeperRepository)
        {
            HouseKeeperRepository = houseKeeperRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor,HouseKeeper")]
        public ActionResult Manage()
        {
            return View("~/Views/HouseKeeping/Manage.cshtml");
        }

        public ActionResult Report(DateTime? from, DateTime? to, int? keeperId, int? roomId)
        {
            DateTime _from = from.HasValue ? new DateTime(from.Value.Year, from.Value.Month, from.Value.Day) : new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            DateTime _to = from.HasValue ? new DateTime(to.Value.Year, to.Value.Month, to.Value.Day) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1);

            ViewBag.From = _from;
            ViewBag.To = _to;
            ViewBag.KeeperID = keeperId;
            ViewBag.Keepers = HouseKeeperRepository.AllHouseKeepers();
            ViewBag.RoomID = roomId;

            var model = HouseKeeperRepository.GetHouseKeepingTrackingReport(_from, _to, keeperId,roomId);
            return View("~/Views/HouseKeeping/Report.cshtml", model);

        }

        // GET: HouseKeeping
        public long RequireClean(RequireCleaningRequest req)
        {
            return HouseKeeperRepository.RequireCleaning(req);
        }

        public void FinishClean(FinishCleaningRequest req)
        {          
            HouseKeeperRepository.FinishCleaning(req);
        }

        public ActionResult GetHouseKeepers()
        {
            return new JsonResult { Data = HouseKeeperRepository.AllHouseKeepers(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor,HouseKeeper")]
        public void DeleteHK(int id)
        {
            HouseKeeperRepository.DeleteHK(id);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor,HouseKeeper")]
        public void AddHK(string name)
        {
            HouseKeeperRepository.AddHK(new HouseKeeper { name = name, create_date = DateTime.Now ,is_active = true });
        }
    }
}