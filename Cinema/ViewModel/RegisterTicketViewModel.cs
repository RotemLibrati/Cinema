using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cinema.ViewModel
{
    public class RegisterTicketViewModel
    {
        public Register register { get; set; }
        public List<Register> UserList { get; set; }
        public Ticket ticket { get; set; }
    }
}