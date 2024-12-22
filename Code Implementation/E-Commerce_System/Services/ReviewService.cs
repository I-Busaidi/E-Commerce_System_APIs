using AutoMapper;
using E_Commerce_System.DTOs.ReviewDTOs;
using E_Commerce_System.Models;
using E_Commerce_System.Repositories;
using System.Security.Cryptography.Xml;

namespace E_Commerce_System.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderProductsService _orderProductsService;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IUserService userService, IProductService productService, IOrderService orderService, IOrderProductsService orderProductsService, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _orderProductsService = orderProductsService;
            _mapper = mapper;
        }

        public List<Review> GetAllReviewsWithRelatedData()
        {
            List<Review> reviews = _reviewRepository.GetAllReviews().ToList();
            if (reviews == null || reviews.Count == 0)
            {
                throw new InvalidOperationException("No reviews found");
            }

            return reviews;
        }

        public List<ReviewOutputDTO> GetAllReviews()
        {
            List<Review> reviews = _reviewRepository.GetAllReviews().ToList();
            if (reviews == null || reviews.Count == 0)
            {
                throw new InvalidOperationException("No reviews found");
            }

            List<ReviewOutputDTO> reviewOutputDTOs = _mapper.Map<List<ReviewOutputDTO>>(reviews);

            return reviewOutputDTOs;
        }

        public List<ReviewOutputDTO> GetProductReviewsByName(string name)
        {
            var product = _productService.GetProductByName(name);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            List<Review> productReviews = _reviewRepository.GetProductReviews(product.productId).ToList();
            if (productReviews == null)
            {
                throw new InvalidOperationException("Product has no reviews");
            }

            List<ReviewOutputDTO> reviewsOutput = _mapper.Map<List<ReviewOutputDTO>>(productReviews);
            return reviewsOutput;
        }

        public List<ReviewOutputDTO> GetReviewsByUserId(int id)
        {
            var user = _userService.GetUserByIdWithRelatedData(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            List<Review> reviewsByUser = _reviewRepository.GetReviewsByUserId(id).ToList();
            if (reviewsByUser == null || reviewsByUser.Count == 0)
            {
                throw new InvalidOperationException("User has not made any reviews");
            }

            List<ReviewOutputDTO> reviewsOutput = _mapper.Map<List<ReviewOutputDTO>>(reviewsByUser);
            return reviewsOutput;
        }

        public int AddReview(ReviewInputWithProductNameDTO reviewInput, int userId)
        {
            if (reviewInput == null)
            {
                throw new ArgumentNullException("Could not add review");
            }
            if (string.IsNullOrWhiteSpace(reviewInput.productName))
            {
                throw new InvalidOperationException("Product not specified");
            }
            if (reviewInput.rating < 1 || reviewInput.rating > 5)
            {
                throw new ArgumentOutOfRangeException("Rating must be from 1 to 5");
            }
            if (string.IsNullOrWhiteSpace(reviewInput.comment))
            {
                reviewInput.comment = "No comment";
            }


            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var product = _productService.GetProductByName(reviewInput.productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            bool checkExistingReview = _reviewRepository.GetReviewsByUserId(userId)
                .Any(r => r.productId == product.productId);

            var userOrders = _orderService.GetUserOrdersWithRelatedData(userId);
            if (userOrders == null)
            {
                throw new InvalidOperationException("Cannot review products not purchased yet.");
            }
            bool userPurchased = false;
            foreach ( var order in userOrders )
            {
                if (order.OrderProducts.Any(op => op.productId == product.productId))
                {
                    userPurchased = true;
                    break;
                }
            }

            if (!userPurchased)
            {
                throw new InvalidOperationException("Cannot review products not purchased yet.");
            }

            if (checkExistingReview)
            {
                throw new InvalidOperationException("User has aleady reviewed this product");
            }
            Review review = new Review
            {
                comment = reviewInput.comment,
                rating = reviewInput.rating,
                userId = userId,
                productId = product.productId
            };
            int productId = _reviewRepository.AddReview(review);

            double newAvgRating = _reviewRepository.GetProductReviews(productId)
                .Average(r => r.rating);

            _productService.UpdateProductRating(product, (decimal)newAvgRating);

            return productId;
        }

        public int UpdateReview(ReviewInputWithProductNameDTO updatesToReview, int userId)
        {
            if (string.IsNullOrWhiteSpace(updatesToReview.productName))
            {
                throw new ArgumentNullException("Product name required");
            }
            if (updatesToReview.rating < 1 || updatesToReview.rating > 5)
            {
                throw new ArgumentOutOfRangeException("Rating must be from 1 to 5");
            }
            if (string.IsNullOrWhiteSpace(updatesToReview.comment))
            {
                updatesToReview.comment = "No Comment";
            }

            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var product = _productService.GetProductByName(updatesToReview.productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var reviewToUpdate = _reviewRepository.GetReviewsByUserId(userId)
                .FirstOrDefault(r => r.productId == product.productId);

            if (reviewToUpdate == null)
            {
                throw new InvalidOperationException("User has not reviewed this product");
            }

            reviewToUpdate.reviewDate = DateTime.Now;
            reviewToUpdate.rating = updatesToReview.rating;
            reviewToUpdate.comment = updatesToReview.comment;

            int updatedReviewId = _reviewRepository.UpdateReview(reviewToUpdate);

            double newAvgRating = _reviewRepository.GetProductReviews(product.productId)
                .Average(r => r.rating);

            _productService.UpdateProductRating(product, (decimal)newAvgRating);

            return updatedReviewId;
        }

        public int DeleteReview(string productName, int userId)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentNullException("Product name is required");
            }
            var product = _productService.GetProductByName(productName);
            if (product == null)
            {
                throw new KeyNotFoundException("product not found");
            }

            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var existingReview = _reviewRepository.GetReviewsByUserId(userId)
                .FirstOrDefault(r => r.productId == product.productId);

            if (existingReview == null)
            {
                throw new InvalidOperationException("Review not found");
            }

            int deletedReviewId = _reviewRepository.DeleteReview(existingReview);

            double? newAvgProductRating;

            if (_reviewRepository.GetProductReviews(product.productId).Any())
            {
                newAvgProductRating = _reviewRepository.GetProductReviews(product.productId).Average(r => r.rating);
            }
            else
            {
                newAvgProductRating = null;
            }

            _productService.UpdateProductRating(product, (decimal?)newAvgProductRating);

            return deletedReviewId;
        }
    }
}
