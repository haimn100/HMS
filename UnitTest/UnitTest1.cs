using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using casa_benjamin.Models;

using System.Linq;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.User.Services;
using casa_benjamin.Modules.User.Entities;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void Setup()
        {
            //CultureInfo info = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            //info.NumberFormat.NumberGroupSeparator = ",";
            //info.NumberFormat.NumberDecimalSeparator = ".";
            //info.NumberFormat.CurrencyGroupSeparator = ",";
            //info.NumberFormat.CurrencyDecimalSeparator = ".";
            //Thread.CurrentThread.CurrentCulture = info;
            CacheManager.Instance.Refresh();
        }

        [TestMethod]
        public void TestNightsCalculation()
        {            
            UIUserBill bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            List<UserBed> beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 28, 21, 14, 0),
                end_date = new DateTime(2017, 12, 30, 0, 40, 0),
                price = 35000                               
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 30, 0, 40, 0),
                end_date = new DateTime(2017, 12, 30, 12, 40, 0),
                price = 35000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill,beds);
            Assert.AreEqual(70000, bill.RoomTotal);
            Assert.AreEqual(2, bill.Rooms.Sum(x=> x.Nights));
        }

        [TestMethod]
        public void TestNightsCalculation2()
        {
            UIUserBill bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            List<UserBed> beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 25, 13, 47, 0),
                end_date = new DateTime(2017, 12, 27, 19, 10, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 27, 19, 10, 0),
                end_date = new DateTime(2017, 12, 28, 0, 41, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 28, 0, 41, 0),
                end_date = new DateTime(2017, 12, 28, 7, 30, 0),
                price = 25000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(3, bill.Rooms.Sum(x => x.Nights));
            
        }

        [TestMethod]
        public void TestNightsCalculation3()
        {
            UIUserBill bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            List<UserBed> beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 25, 13, 47, 0),
                end_date = new DateTime(2017, 12, 27, 19, 10, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 27, 19, 10, 0),
                end_date = new DateTime(2017, 12, 28, 0, 41, 0),
                price = 35000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 28, 0, 41, 0),
                end_date = new DateTime(2017, 12, 28, 14, 30, 0),
                price = 35000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(75000, bill.RoomTotal);
            Assert.AreEqual(3, bill.Rooms.Sum(x => x.Nights));
        }


        [TestMethod]
        public void TestNightsCalculation4()
        {
            UIUserBill bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            List<UserBed> beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2018, 8, 29, 21, 12, 0),
                end_date = new DateTime(2018, 9, 1, 16, 45, 0),
                price = 2000
            });

            //less than minimum hours 
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(8000, bill.RoomTotal);
            Assert.AreEqual(4, bill.Rooms.Sum(x => x.Nights));


            bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 1, 3, 0, 0),
                end_date = new DateTime(2017, 12, 2, 3, 0, 0),
                price = 25000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(50000, bill.RoomTotal);
            Assert.AreEqual(2, bill.Rooms.Sum(x => x.Nights));


            bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 1, 3, 0, 0),
                end_date = new DateTime(2017, 12, 2, 3, 0, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 2, 3, 0, 0),
                end_date = new DateTime(2017, 12, 2, 10, 0, 0),
                price = 25000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(2, bill.Rooms.Sum(x => x.Nights));

            bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 1, 3, 0, 0),
                end_date = new DateTime(2017, 12, 2, 3, 0, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 2, 3, 0, 0),
                end_date = new DateTime(2017, 12, 2, 10, 0, 0),
                price = 25000
            });
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2017, 12, 2, 10, 0, 0),
                end_date = new DateTime(2017, 12, 2, 13, 0, 0),
                price = 35000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(50000, bill.RoomTotal);
            Assert.AreEqual(2, bill.Rooms.Sum(x => x.Nights));


            bill = new UIUserBill();
            bill.Rooms = new List<RoomRow>();
            beds = new List<UserBed>();
            beds.Add(new UserBed
            {
                bed_id = 1,
                start_date = new DateTime(2018, 1, 9, 9, 52, 0),
                end_date = new DateTime(2018, 1, 15, 22, 48, 0),
                price = 25000
            });
            UserManager.Instance.CalculateRoomPricesNew(bill, beds);
            Assert.AreEqual(7, bill.Rooms.Sum(x => x.Nights));
        }
    }
}
