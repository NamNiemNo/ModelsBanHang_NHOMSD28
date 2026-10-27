using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class ChucVuRepository : IChucVuRepository
    {
        private readonly DbContextApp _db;

        public ChucVuRepository(DbContextApp db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ChucVu>> GetAllChucVuAsync()
        {
            return await _db.ChucVus.ToListAsync();
        }

        public async Task<ChucVu> GetByIdChucVuAsync(Guid id)
        {
            return await _db.ChucVus.FindAsync(id);
        }

        public async Task CreateChucVuAsync(ChucVu chucVu)
        {
            try
            {
                _db.ChucVus.Add(chucVu);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateChucVuAsync(ChucVu chucVu)
        {
            try
            {
                _db.ChucVus.Update(chucVu);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteChucVuAsync(Guid id)
        {
            try
            {
                var chucVu = await _db.ChucVus.FindAsync(id);
                if (chucVu != null)
                {
                    _db.ChucVus.Remove(chucVu);
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}