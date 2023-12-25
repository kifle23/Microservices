using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo(AppDbContext context) : IPlatformRepo
    {
        private readonly AppDbContext _context = context;

        public void CreatePlatform(Platform plat)
        {
            if (plat != null)
            {
                _context.Platforms.Add(plat);
            }
            else
            {
                throw new ArgumentNullException(nameof(plat));
            }
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int id) => _context.Platforms.FirstOrDefault(p => p.Id == id) ?? throw new ArgumentNullException(nameof(id));

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
