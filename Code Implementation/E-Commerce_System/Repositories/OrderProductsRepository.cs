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

        public int AddOrderProduct(List<OrderProducts> products)
        {
            _context.OrdersProducts.AddRange(products);
            _context.SaveChanges();
            return products.Count();
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
