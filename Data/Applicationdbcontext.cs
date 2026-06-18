using CareProjct.web.Models;
using Microsoft.EntityFrameworkCore;
using CareProjct.web.Models;

namespace CareProjct.web.Data
{
    public class Applicationdbcontext : DbContext
    {
        public Applicationdbcontext(DbContextOptions<Applicationdbcontext> options) : base(options)
        {
        }

        public DbSet<Register>           Register          { get; set; }
        public DbSet<Caretaker>          Caretaker         { get; set; }
        //public DbSet<PaymentInfo>        PaymentInfo       { get; set; }
        public DbSet<FeedbackViewModel>  FeedbackViewModel { get; set; }
        //public DbSet<Orders1>            Orders1           { get; set; }
        //public DbSet<OrderItems>         OrderItems        { get; set; }
        public DbSet<OrderConfirm>       OrderConfirm      { get; set; }
    }
}