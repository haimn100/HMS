using casa_benjamin.Modules.HouseKeeping.Data;
using casa_benjamin.Modules.Shared.Repositories;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace casa_benjamin
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            container.RegisterSingleton<IGenericRepository,GenericRepository>();
            container.RegisterSingleton<IHouseKeeperRepository, HouseKeepingRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}