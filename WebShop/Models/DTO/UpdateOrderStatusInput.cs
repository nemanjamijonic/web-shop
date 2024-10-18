using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Enums;

namespace WebShop.Models.DTO
{
    public class UpdateOrderStatusInput
    {
        public Guid Id { get; set; }
        public OrderStatusInputEnum Status { get; set; }
    }
}