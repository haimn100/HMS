using casa_benjamin.Modules.Shared.Services;

namespace casa_benjamin.Helpers
{
    public static class UserHelper
    {
        public static bool IsResident(int userId)
        {
            return CacheManager.Instance.Residents.Exists(x => x.id == userId);
        }

    }
}