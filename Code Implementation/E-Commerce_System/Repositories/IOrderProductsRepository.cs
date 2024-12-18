using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IOrderProductsRepository
    {
        List<OrderProducts> AddOrderProduct(List<OrderProducts> products);
        void DeleteOrderProduct(OrderProducts orderProduct);
        void UpdateOrderProduct(OrderProducts orderProduct);
    }
}