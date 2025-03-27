using PartialView.pustok.DATA;
using PartialView.pustok.Models;

namespace PartialView.pustok.Services
{
    public class LayoutService(PustokDbContext _pustokDbContext)
    {
        public List<Setting> GetSettings()
        {
            return _pustokDbContext.Settings.ToList();
        }

        public List<Genre> GetGenres()
        {
            return _pustokDbContext.Genres.ToList();
        }
    }
}
