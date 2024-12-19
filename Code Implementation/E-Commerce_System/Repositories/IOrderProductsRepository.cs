using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IOrderProductsRepository
    {
        OrderProducts AddOrderProduct(OrderProducts product);
        void DeleteOrderProduct(OrderProducts orderProduct);
        void UpdateOrderProduct(OrderProducts orderProduct);
    }
}