using Microsoft.AspNetCore.Mvc.Rendering;
using ReadModel.Cart;
using ReadModel.Customer;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Models
{
    public class CartIndexViewModel
    {
        public IEnumerable<Cart> Carts { get; set; }

        public IEnumerable<Customer> Customers { get; set; }

        public IEnumerable<SelectListItem> AvailableCustomers
        {
            get
            {
                return Customers.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                })
                .ToList();
            }
        }
    }
}
