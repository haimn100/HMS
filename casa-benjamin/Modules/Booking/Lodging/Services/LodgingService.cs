using casa_benjamin.Modules.Booking.Lodging.Enums;
using casa_benjamin.Modules.Shared.Repositories;

namespace casa_benjamin.Modules.Booking.Lodging.Services
{
    public class LodgingService
    {
        private GenericRepository repository;
        private const string TABLE = "reservation_lodging";

        public LodgingService(string dbConnectionString)
        {
            repository = new GenericRepository();
        }

        public long Add(Lodging.Entities.Lodging item)
        {
            return repository.Insert(item);
        }

        public void ChangeStatus(int lodgingId, LodgingStatus status)
        {
            repository.ExecuteScalar($"update {TABLE} set status = {status} where id = {lodgingId}");
        }

        public void ChangeDates(int lodgingId, LodgingStatus status)
        {
            repository.ExecuteScalar($"update {TABLE} set status = {status} where id = {lodgingId}");
        }

    }
}