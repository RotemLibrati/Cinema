using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Cinema.Dal
{
    public class GalleryMovieDal: DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GalleryMovie>().ToTable("GalleryMovie");
        }
        public DbSet<GalleryMovie> GalleryMovie { get; set; }
    }
}