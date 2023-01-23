using casa_benjamin.Data;
using casa_benjamin.Models;
 
using casa_benjamin.Modules.App.Values;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.User.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.User.Services;

namespace casa_benjamin.Modules.Shared.Services
{
    public sealed class CacheManager
    {
        private static volatile CacheManager instance;
        private static object syncRoot = new Object();

        private UserRepository userRepository;
        private KitchenRepository kitchenRepository;
        GenericRepository genericRepository;


        List<UIMenuCategory> _MenuCategories;
        List<Modules.User.Entities.User> _Users;
        List<Staff.Entities.Staff> _Staff;
        List<UIRoom> _Rooms;
        List<Reservation> _Rerervations;
        List<Ingredient> _Ingredients;
        List<StockItem> _StockAlerts;
        User.Entities.User _GhostUser;
        AppSettings _AppSettings;
        List<User.Entities.User> _Residents;


        public List<UIMenuCategory> MenuCategories { get { return _MenuCategories; } }
        public List<Modules.User.Entities.User> Users { get { return _Users; } }
        public List<Reservation> Reservations { get { return _Rerervations; } }

        public List<Staff.Entities.Staff> Staff { get { return _Staff; } }
        public List<UIRoom> Rooms { get { return _Rooms; } }
        public List<Ingredient> Ingredients { get { return _Ingredients; } }
        public List<StockItem> StockAlerts { get { return _StockAlerts; } }
        public AppSettings AppSettings { get { return _AppSettings; } }
        public List<Modules.User.Entities.User> Residents { get { return _Residents; } }

        public User.Entities.User GhostUser { get { return _GhostUser; } }

        private CacheManager()
        {
            userRepository = new UserRepository();
            kitchenRepository = new KitchenRepository();
            genericRepository = new GenericRepository();
            CreateGhostUserAndBedIfNeeded();
        }

        public static CacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CacheManager();
                    }
                }

                return instance;
            }
        }

        public void Refresh()
        {
            RefreshCache();
        }

        public void RefreshBeds()
        {
            List<Bed> beds = UserManager.Instance.GetBeds();
            List<RoomBed> roomBeds = UserManager.Instance.GetRoomBeds();

            foreach (var item in roomBeds)
            {
                Bed bed = beds.First(x => x.id == item.bed_id);
                if(bed.bed_type_id != item.bed_type_id)
                {
                    item.bed_type_id = bed.bed_type_id;
                    item.double_bed_partner_id = bed.double_bed_partner_id;
                    UserManager.Instance.UpdateRoomBed(item);
                }
            }

        }
        
        public void RefreshRooms()
        {
            List<Room> rooms = UserManager.Instance.GetRooms();

            List<UIRoom> uiRooms = new List<UIRoom>();
            var roomBeds = UserManager.Instance.GetRoomBeds();

            foreach (var rb in roomBeds.OrderBy(x=> x.room_id).GroupBy(x => x.room_id))
            {
                var uiRoom = new UIRoom
                {
                    room = rooms.First(x => x.id == rb.Key),
                    beds = rb.OrderBy(x=> x.bed_id).ToList()
                };

                uiRooms.Add(uiRoom);
            }
            _Rooms = uiRooms;
        }

        public void RefreshStaff()
        {
            List<Staff.Entities.Staff> staff = userRepository.GetStaff();
            _Staff = staff;
        }

        public void RefreshUsers()
        {
            List<User.Entities.User> users = userRepository.GetAllCheckedInUsers();
            _Users = new List<Modules.User.Entities.User>();
            _Users = users;           
        }

        public void RefreshCategories()
        {
            List<MenuCategory> menuCategories = genericRepository.Get<MenuCategory>("select * from menu_category where is_active = 1 order by number asc");
            List<MenuItem> menuItems = genericRepository.Get<MenuItem>("select * from menu_item where is_active = 1");
            List<Ingredient> ingredients = genericRepository.Get<Ingredient>("select * from ingredient");
            List<MenuItemIngredient> menuItemIngredients = genericRepository.Get<MenuItemIngredient>("select * from menu_item_ingredient");


            _MenuCategories = new List<UIMenuCategory>();
            //create uimenucategories
            foreach (var menuCategory in menuCategories)
            {
                List<MenuItem> catMenuItems = menuItems.Where(x => x.cat_id == menuCategory.id).OrderBy(x=> x.number).ToList();
                List<UIMenuItem> uiMenuItems = new List<UIMenuItem>();

                foreach (var mi in catMenuItems)
                {
                    uiMenuItems.Add(new UIMenuItem(mi, menuItemIngredients.Where(x => x.menu_item_id == mi.id).ToList(), menuCategory));
                }
                UIMenuCategory cat = new UIMenuCategory(menuCategory, uiMenuItems);
                _MenuCategories.Add(cat);
            }

            //Ingredients
            _Ingredients = new List<Ingredient>();
            _Ingredients = ingredients;
        }

        public void RefreshAppSettings()
        {
            AppSettings app = genericRepository.Get<AppSettings>("select * from app_settings").First();
            _AppSettings = app;
        }   

        public void RefreshResidents()
        {
            List<Modules.User.Entities.User> users = userRepository.GetAllResidents();
            _Residents = users;
        }

        public RoomBed GetRoomBed(int bedId)
        {
            try
            {
                foreach (var uiRoom in Rooms)
                {
                    RoomBed rb = uiRoom.beds.FirstOrDefault(x => x.bed_id == bedId);
                    if (rb != null)
                    {
                        return rb;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return null;
        }

        public Room GetRoom(int roomId)
        {
            try
            {
                UIRoom uiRoom = Rooms.FirstOrDefault(x => x.room.id == roomId);
                if(uiRoom != null)
                {
                    return uiRoom.room;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return null;
        }

        public UIRoom GetUIRoom(int roomNumber)
        {
            try
            {
                UIRoom uiRoom = Rooms.FirstOrDefault(x => x.room.room_number == roomNumber || x.room.id == roomNumber);
                if (uiRoom != null)
                {
                    return uiRoom;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return null;
        }

        public Room GetRoomByBed(int bedId)
        {
            try
            {
                UIRoom uiRoom = Rooms.FirstOrDefault(x => x.beds.Select(k=>k.bed_id).Contains(bedId));
                if (uiRoom != null)
                {
                    return uiRoom.room;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return null;
        }

        private void RefreshCache()
        {            
            RefreshUsers();
            RefreshStaff();
            RefreshRooms();
            RefreshCategories();           
            RefreshAppSettings();
            RefreshResidents();
        }

        private void CreateGhostUserAndBedIfNeeded()
        {
           User.Entities.User user = userRepository.GetGhostUser();
           Bed bed = userRepository.GetGhostBed();
          
            if (bed == null)
            {
                userRepository.CreateGhostBed();
                bed = userRepository.GetGhostBed();
            }

            if (user == null)
            {             
                userRepository.CreateGhostUser(bed.id);
                user = userRepository.GetGhostUser();
            }

            _GhostUser = user;

        }

    }
}