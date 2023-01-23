using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.User.Entities;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIUserBill
    {
        public User User { get; set; }
        public List<OrderRow> Orders { get; set; }
        public List<RoomRow> Rooms { get; set; }
        public double OrdersTotal { get; set; }
        public double KitchenTotal { get; set; }
        public double ServicesTotal { get; set; }    
        public double RoomTotal { get; set; }
        public double DiscountTotal { get; set; }
        public decimal TotalDepositsCreditCardCharge { get; set; }
        public double Total { get; set; }
        public double SubTotal { get; set; }
        public List<UserDiscount> Discounts { get; set; }
        public List<UserPrePay> Deposits { get; set; }
        public List<RoomNight> Nights { get; set; }
        public List<UserBed> UserBeds { get; set; }
    }

    public class OrderRow
    {
       public Order Order { get; set; }
       public List<OrderItems> OrderItems { get; set; }
       public string SplitedByUserName { get; set; }
       public int SplitedByUserId { get; set; }
    }

    public class RoomNight
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public int BedId { get; set; }
        public bool FullNight { get; set; }
    }

    public class RoomRow
    {
       
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BedId { get; set; }
        public int RoomNumber { get; set; }
        public double Price { get; set; }
        public int Nights { get; set; }
    }
}