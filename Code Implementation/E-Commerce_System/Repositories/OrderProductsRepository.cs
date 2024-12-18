using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public class OrderProductsRepository : IOrderProductsRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderProductsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<OrderProducts> AddOrderProduct(List<OrderProducts> products)
        {
            _context.OrdersProducts.AddRange(products);
            return products;
        }

        public void DeleteOrderProduct(OrderProducts orderProduct)
        {
            _context.OrdersProducts.Remove(orderProduct);
            _context.SaveChanges();
        }

        public void UpdateOrderProduct(OrderProducts orderProduct)
        {
            _context.OrdersProducts.Remove(orderProduct);
            _context.SaveChanges();
        }
    }
}
