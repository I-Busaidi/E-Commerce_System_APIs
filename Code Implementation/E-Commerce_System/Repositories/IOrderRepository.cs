using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IOrderRepository
    {
        Order AddOrder(Order order);
        void DeleteOrder(Order order);
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int id);
        IEnumerable<Order> GetOrdersByUserId(int id);
        Order UpdateOrder(Order order);
    }
}