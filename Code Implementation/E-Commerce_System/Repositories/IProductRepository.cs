using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IProductRepository
    {
        Product AddProduct(Product product);
        void DeleteProduct(Product product);
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        Product UpdateProduct(Product product);
        void StockUpdateProduct(Product product);
    }
}