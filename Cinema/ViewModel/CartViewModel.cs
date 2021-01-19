using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cinema.ViewModel
{
    public class CartViewModel
    {
        public Cart cart { get; set; }
        public string nameUser { get; set; }
        public List<Cart> carts { get; set; }
        public int Sum { get; set; }
    }
}