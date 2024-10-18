namespace WebShop.Models.Domain
{
    using System;
    using WebShop.Models.Enums;

    public class Order
    {
        public Order()
        {

        }
        public Order(Guid productId, string name, double price, int quantity, string description, string imageURL, DateTime postDate, string city, bool isAvaliable, bool isDeleted, int orderQuantity,
            string username, string password, string customerName, string surname, Gender gender, string email, DateTime birthDate, bool isUserDeleted, DateTime orderDate, OrderState orderState)
        {
            Id = Guid.NewGuid();
            Product = new Product(productId, name, price, quantity, description, imageURL, postDate, city, isAvaliable, isDeleted);
            Quantity = orderQuantity;
            Customer = new Customer(username, password, customerName, surname, gender, email, birthDate, isUserDeleted);
            OrderDate = orderDate;
            OrderState = orderState;

        }

        public Order(Product product, int quantity, Customer customer)
        {
            Id = Guid.NewGuid();
            Product = product;
            Quantity = quantity;
            Customer = customer;
            OrderDate = DateTime.Now;
            OrderState = OrderState.Active;
        }

        public override string ToString()
        {
            return Id.ToString() + "|" + Product.ToString() + $"|{Quantity}|" + Customer.ToString() + $"|{OrderDate.ToString("dd-MM-yyyy")}|{OrderState}|{IsDeleted}";
        }


        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderState OrderState { get; set; }
        public bool IsDeleted { get; set; }

    }
}