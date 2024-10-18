using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models.DTO
{
    public class OrderProductInput
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}