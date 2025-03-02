using Microsoft.EntityFrameworkCore;

namespace PartialView.pustok.DATA
{
    public class PustokDbContext:DbContext
    {
        public PustokDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
