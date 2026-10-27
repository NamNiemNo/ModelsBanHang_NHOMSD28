using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace API.IRepository.Repository
{
    public class AnhRepository : IAnhRepository
    {
        private readonly DbContextApp _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AnhRepository> _logger;
        private readonly string _uploadFolderPath;
        private const string _relativePath = "/Uploads/Images";
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public AnhRepository(DbContextApp context, IWebHostEnvironment env, ILogger<AnhRepository> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _uploadFolderPath = Path.Combine(_env?.WebRootPath ?? string.Empty, "uploads", "images");
            Directory.CreateDirectory(_uploadFolderPath); // Đảm bảo thư mục tồn tại
        }

        public async Task<IEnumerable<Anh>> GetAllAsync()
        {
            return await _context.Anhs.AsNoTracking().ToListAsync();
        }

        public async Task<Anh?> GetByIdAsync(Guid id)
        {
            return await _context.Anhs.FindAsync(id);
        }

        public async Task<Anh?> UploadAsync(IFormFile file, string tenAnh, Guid giayChiTietId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate file
                if (!ValidateFile(file, out var extension))
                    return null;

                // Generate unique filename
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(_uploadFolderPath, fileName);

                // Save file
                await SaveFileAsync(file, fullPath);

                // Create database record
                var anh = new Anh
                {
                    TenAnh = tenAnh,
                    DuongDan = $"{_relativePath}/{fileName}",
                    TrangThai = true,
                    GiayChiTietId = giayChiTietId
                };

                _context.Anhs.Add(anh);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return anh;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi khi upload ảnh");
                throw;
            }
        }

        public async Task<Anh?> UpdateAsync(Anh anh)
        {
            try
            {
                var existing = await _context.Anhs.FindAsync(anh.AnhId);
                if (existing == null) return null;

                existing.TenAnh = anh.TenAnh;
                existing.TrangThai = anh.TrangThai;
                existing.GiayChiTietId = anh.GiayChiTietId;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật ảnh ID: {anh.AnhId}");
                throw;
            }
        }

        public async Task<Anh?> UpdateFileAsync(Guid id, IFormFile file, string tenAnh, Guid giayChiTietId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate file
                if (!ValidateFile(file, out var extension))
                    return null;

                var existing = await _context.Anhs.FindAsync(id);
                if (existing == null) return null;

                // Delete old file if exists
                await DeletePhysicalFileAsync(existing.DuongDan);

                // Save new file
                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(_uploadFolderPath, fileName);
                await SaveFileAsync(file, fullPath);

                // Update record
                existing.DuongDan = $"{_relativePath}/{fileName}";
                existing.TenAnh = tenAnh ?? existing.TenAnh;
                existing.GiayChiTietId = giayChiTietId;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return existing;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Lỗi khi cập nhật file ảnh ID: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var anh = await _context.Anhs.FindAsync(id);
                if (anh == null) return false;

                // Delete physical file
                await DeletePhysicalFileAsync(anh.DuongDan);

                // Delete database record
                _context.Anhs.Remove(anh);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Lỗi khi xóa ảnh ID: {id}");
                throw;
            }
        }

        private bool ValidateFile(IFormFile file, out string extension)
        {
            extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (file.Length == 0 || file.Length > _maxFileSize)
                return false;

            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                return false;

            return true;
        }

        private async Task SaveFileAsync(IFormFile file, string fullPath)
        {
            try
            {
                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lưu file: {fullPath}");
                throw;
            }
        }

        private async Task DeletePhysicalFileAsync(string relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath)) return;

                var fileName = Path.GetFileName(relativePath);
                if (string.IsNullOrEmpty(fileName)) return;

                var fullPath = Path.Combine(_uploadFolderPath, fileName);
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa file vật lý: {relativePath}");
                throw;
            }
        }
    }
}