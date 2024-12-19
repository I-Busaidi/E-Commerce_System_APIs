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

        public OrderProducts AddOrderProduct(OrderProducts product)
        {
            _context.OrdersProducts.Add(product);
            return product;
        }

        public IEnumerable<OrderProducts> GetOrdersProductsById(int id)
        {
            return _context.OrdersProducts.Where(op => op.orderId == id);
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
