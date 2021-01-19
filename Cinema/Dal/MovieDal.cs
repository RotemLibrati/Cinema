using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Cinema.Dal
{
    public class MovieDal: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>().ToTable("Movie");
        }
        public DbSet<Movie> Movie { get; set; }

        //public System.Data.Entity.DbSet<Cinema.Models.Movie> movies { get; set; }
    }
}