using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {
        // for ef core 
        private Order()
        {
            
        }
        public Order(string userEmail, OrderAddress shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal)
        {
            UserEmail = userEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
        }

        public string UserEmail { get; set; } = default!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // it is pending by default
        public OrderAddress ShippingAddress { get; set; } = default!; // owned entity 
        public DeliveryMethod DeliveryMethod { get; set; } = default!;
        public int DeliveryMethodId { get; set; } // FK
        public ICollection<OrderItem> Items { get; set; } = [];
        public decimal SubTotal { get; set; }
        //public decimal Total { get
        //    {
        //        return SubTotal + DeliveryMethod.Cost;
        //    }
        //} // computed derived property 

        public decimal GetTotal() => SubTotal + (DeliveryMethod?.Cost ?? 0);

    }
}
