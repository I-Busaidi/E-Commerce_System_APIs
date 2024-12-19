using E_Commerce_System.Models;

namespace E_Commerce_System.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _context.Reviews;
        }

        public IEnumerable<Review> GetProductReviews(int id)
        {
            return _context.Reviews.Where(r => r.productId == id);
        }

        public IEnumerable<Review> GetReviewsByUserId(int userId)
        {
            return _context.Reviews.Where(r => r.userId == userId);
        }

        public Review GetReviewById(int id)
        {
            return _context.Reviews.Find(id);
        }

        public int AddReview(Review review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
            return review.productId;
        }

        public int UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
            return review.productId;
        }

        public int DeleteReview(Review review)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return review.productId;
        }
    }
}
