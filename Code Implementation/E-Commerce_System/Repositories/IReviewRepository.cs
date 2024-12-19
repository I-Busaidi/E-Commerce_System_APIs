using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public interface IReviewRepository
    {
        int AddReview(Review review);
        int DeleteReview(Review review);
        IEnumerable<Review> GetAllReviews();
        IEnumerable<Review> GetProductReviews(int id);
        Review GetReviewById(int id);
        IEnumerable<Review> GetReviewsByUserId(int userId);
        int UpdateReview(Review review);
    }
}