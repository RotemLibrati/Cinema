using Cinema.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Cinema.Dal
{
    public class RegisterUserDal : DbContext
    {
        public DbSet<Register> RegisterUser {get; set;}
        public DbSet<LoginViewModel> LoginViewModels { get; set; }
        public object Register { get; internal set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Register>().ToTable("RegisterUser");
        }
    }
}