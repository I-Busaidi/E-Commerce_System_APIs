using E_Commerce_System.DTOs.ReviewDTOs;
using E_Commerce_System.Models;

namespace E_Commerce_System.Services
{
    public interface IReviewService
    {
        int AddReview(ReviewInputWithProductNameDTO reviewInput, int userId);
        int DeleteReview(string productName, int userId);
        List<ReviewOutputDTO> GetAllReviews();
        List<Review> GetAllReviewsWithRelatedData();
        List<ReviewOutputDTO> GetProductReviewsByName(string name);
        List<ReviewOutputDTO> GetReviewsByUserId(int id);
        int UpdateReview(ReviewInputWithProductNameDTO updatesToReview, int userId);
    }
}