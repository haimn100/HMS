using casa_benjamin.Modules.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace casa_benjamin.Modules.Staff.Services
{

   
    public class StaffService
    {
        private readonly GenericRepository repository;
        private const string ROOMS_TABLE = "room";

        public StaffService(string dbConnectionString)
        {
            repository = new GenericRepository();
        }

        public List<Entities.Staff> All()
        {
            return repository.GetAll<Entities.Staff>();
        }
    }
}