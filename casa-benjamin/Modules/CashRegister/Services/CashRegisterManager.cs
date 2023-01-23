using System;
using System.Collections.Generic;
using System.Linq;
using casa_benjamin.Helpers;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Shared.Repositories;

namespace casa_benjamin.Modules.CashRegister.Services
{
    public class CashRegisterManager
    {
        private static volatile CashRegisterManager instance;
        private static object syncRoot = new Object();

        private GenericRepository genericRepository;


        private CashRegisterManager()
        {
            genericRepository = new GenericRepository();
        }

        public static CashRegisterManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CashRegisterManager();
                    }
                }

                return instance;
            }
        }

        public List<CashRegisterEvent> GetRegisterEvents(DateTime from, DateTime? to)
        {
            if(to.HasValue)
            {
                string startPeriodString = from.ToMySqlDateTimeString();
                string endPeriodString = to.Value.ToMySqlDateTimeString();

                return genericRepository.Get<CashRegisterEvent>(string.Format("select * from cash_register_event where event_date between '{0}' and '{1}'", startPeriodString, endPeriodString)).ToList();
            }
            else
            {
                string startPeriodString = from.ToMySqlDateTimeString();
                return genericRepository.Get<CashRegisterEvent>(string.Format("select * from cash_register_event where event_date > '{0}'", startPeriodString)).ToList();
            }
        }

        public List<CashRegisterEvent> GetRegisterEvents(int quantity, int offset)
        {            
            return genericRepository.Get<CashRegisterEvent>(string.Format("select * from cash_register_event order by id desc limit {0},{1}", offset,quantity)).ToList();
        }

        /// <summary>
        /// If Last Event is null then a stub event returns
        /// Default > { current_register_amount = 0, event_date = now }
        /// </summary>
        /// <returns></returns>
        public CashRegisterEvent GetLastEventOrDefault()
        {
            var response = genericRepository.Get<CashRegisterEvent>("select * from cash_register_event order by id desc limit 1").FirstOrDefault();
            return response != null ? response : new CashRegisterEvent { current_register_amount = 0, event_date = DateTimeHelper.GetCurrentDateTime() };
        }

        public void UpdateEvent(CashRegisterEvent ev)
        {
            genericRepository.Update(ev);
        }

        public void AddEvent(CashRegisterEvent ev)
        {
            genericRepository.Insert(ev);
        }

    }
}