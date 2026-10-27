using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class DeGiayRepository : IDeGiayRepository
    {
        private readonly DbContextApp _db;

        public DeGiayRepository(DbContextApp db)
        {
            _db = db;
        }

        public async Task DeleteDeGiay(Guid id)
        {
            try
            {
                var deldg = await _db.DeGiays.FindAsync(id);
                if (deldg != null)
                {
                    _db.DeGiays.Remove(deldg);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task CreateDeGiay(DeGiay deGiay)
        {
            try
            {
                _db.Add(deGiay);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<DeGiay> GetDeGiay(Guid? id)
        {
            return await _db.DeGiays.FindAsync(id);
        }
        public async Task UpdateDeGiay(DeGiay deGiay)
        {
            try
            {
                _db.DeGiays.Update(deGiay);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<DeGiay>> GetAllDegiay()
        {
            return await _db.DeGiays.ToListAsync();
        }
    }

}
