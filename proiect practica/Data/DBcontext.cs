using Microsoft.EntityFrameworkCore;
using proiect_practica.Models;

namespace proiect_practica.Data
{
    public class DBcontext:DbContext
    {
        public DBcontext(DbContextOptions<DBcontext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }


    }
}
