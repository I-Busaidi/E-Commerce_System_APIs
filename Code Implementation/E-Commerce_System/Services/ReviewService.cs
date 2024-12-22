using AutoMapper; // AutoMapper is used to map between domain models and DTOs.
using E_Commerce_System.DTOs.ReviewDTOs; // DTOs for review-related data transfer.
using E_Commerce_System.Models; // Domain models like Review, Product, User, etc.
using E_Commerce_System.Repositories; // Interfaces for data access and repository pattern.
using System.Security.Cryptography.Xml;

namespace E_Commerce_System.Services
{
    // Service class that provides business logic related to reviews.
    public class ReviewService : IReviewService
    {
        // Dependencies injected via constructor
        private readonly IReviewRepository _reviewRepository; // Repository for review-related data operations.
        private readonly IUserService _userService; // Service for user-related operations.
        private readonly IProductService _productService; // Service for product-related operations.
        private readonly IOrderService _orderService; // Service for order-related operations.
        private readonly IOrderProductsService _orderProductsService; // Service for order-product related operations.
        private readonly IMapper _mapper; // AutoMapper instance for mapping entities to DTOs.

        // Constructor that initializes dependencies.
        public ReviewService(IReviewRepository reviewRepository, IUserService userService, IProductService productService, IOrderService orderService, IOrderProductsService orderProductsService, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _orderProductsService = orderProductsService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all reviews with related data from the repository.
        /// </summary>
        /// <returns>A list of Review entities.</returns>
        public List<Review> GetAllReviewsWithRelatedData()
        {
            List<Review> reviews = _reviewRepository.GetAllReviews().ToList();
            if (reviews == null || reviews.Count == 0)
            {
                throw new InvalidOperationException("No reviews found");
            }

            return reviews;
        }

        /// <summary>
        /// Retrieves all reviews and maps them to ReviewOutputDTO.
        /// </summary>
        /// <returns>A list of ReviewOutputDTO.</returns>
        public List<ReviewOutputDTO> GetAllReviews()
        {
            // Get all reviews and map them to DTO.
            List<Review> reviews = _reviewRepository.GetAllReviews().ToList();
            if (reviews == null || reviews.Count == 0)
            {
                throw new InvalidOperationException("No reviews found");
            }

            List<ReviewOutputDTO> reviewOutputDTOs = _mapper.Map<List<ReviewOutputDTO>>(reviews);
            return reviewOutputDTOs;
        }

        /// <summary>
        /// Retrieves reviews for a specific product by its name.
        /// </summary>
        /// <param name="name">Product name to fetch reviews for.</param>
        /// <returns>A list of ReviewOutputDTO for the given product.</returns>
        public List<ReviewOutputDTO> GetProductReviewsByName(string name)
        {
            // Get the product by name to fetch its reviews.
            var product = _productService.GetProductByName(name);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Get all reviews for the specified product.
            List<Review> productReviews = _reviewRepository.GetProductReviews(product.productId).ToList();
            if (productReviews == null || productReviews.Count == 0)
            {
                throw new InvalidOperationException("Product has no reviews");
            }

            // Map the reviews to DTO and return.
            List<ReviewOutputDTO> reviewsOutput = _mapper.Map<List<ReviewOutputDTO>>(productReviews);
            return reviewsOutput;
        }

        /// <summary>
        /// Retrieves all reviews made by a specific user by their ID.
        /// </summary>
        /// <param name="id">User ID to fetch reviews by.</param>
        /// <returns>A list of ReviewOutputDTO made by the user.</returns>
        public List<ReviewOutputDTO> GetReviewsByUserId(int id)
        {
            // Get the user by ID to check their reviews.
            var user = _userService.GetUserByIdWithRelatedData(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Get all reviews by the user.
            List<Review> reviewsByUser = _reviewRepository.GetReviewsByUserId(id).ToList();
            if (reviewsByUser == null || reviewsByUser.Count == 0)
            {
                throw new InvalidOperationException("User has not made any reviews");
            }

            // Map the reviews to DTO and return.
            List<ReviewOutputDTO> reviewsOutput = _mapper.Map<List<ReviewOutputDTO>>(reviewsByUser);
            return reviewsOutput;
        }

        /// <summary>
        /// Adds a new review for a product from a user.
        /// </summary>
        /// <param name="reviewInput">Input DTO containing review details (rating, comment, product name).</param>
        /// <param name="userId">The user ID who is adding the review.</param>
        /// <returns>The ID of the newly added review.</returns>
        public int AddReview(ReviewInputWithProductNameDTO reviewInput, int userId)
        {
            // Validate input data for review.
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
                reviewInput.comment = "No comment"; // Default to "No comment" if no comment is provided.
            }

            // Check if the user exists.
            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Get the product by name and validate if it exists.
            var product = _productService.GetProductByName(reviewInput.productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check if the user has already reviewed the product.
            bool checkExistingReview = _reviewRepository.GetReviewsByUserId(userId)
                .Any(r => r.productId == product.productId);

            // Get user's orders to verify if they purchased the product.
            var userOrders = _orderService.GetUserOrdersWithRelatedData(userId);
            if (userOrders == null)
            {
                throw new InvalidOperationException("Cannot review products not purchased yet.");
            }

            bool userPurchased = false;
            foreach (var order in userOrders)
            {
                if (order.OrderProducts.Any(op => op.productId == product.productId))
                {
                    userPurchased = true;
                    break;
                }
            }

            // If the user has not purchased the product, they cannot review it.
            if (!userPurchased)
            {
                throw new InvalidOperationException("Cannot review products not purchased yet.");
            }

            // If the user has already reviewed the product, prevent adding another review.
            if (checkExistingReview)
            {
                throw new InvalidOperationException("User has already reviewed this product");
            }

            // Create a new review entity and save it to the repository.
            Review review = new Review
            {
                comment = reviewInput.comment,
                rating = reviewInput.rating,
                userId = userId,
                productId = product.productId
            };

            // Add review and get the product ID.
            int productId = _reviewRepository.AddReview(review);

            // Recalculate the product's average rating.
            double newAvgRating = _reviewRepository.GetProductReviews(productId)
                .Average(r => r.rating);

            // Update the product's rating based on the new average.
            _productService.UpdateProductRating(product, (decimal)newAvgRating);

            return productId;
        }

        /// <summary>
        /// Updates an existing review for a product by the user.
        /// </summary>
        /// <param name="updatesToReview">DTO containing updated review details.</param>
        /// <param name="userId">The user ID who is updating the review.</param>
        /// <returns>The ID of the updated review.</returns>
        public int UpdateReview(ReviewInputWithProductNameDTO updatesToReview, int userId)
        {
            // Validate the input data for review update.
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
                updatesToReview.comment = "No Comment"; // Default to "No Comment" if no comment is provided.
            }

            // Check if the user exists.
            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Get the product by name and validate if it exists.
            var product = _productService.GetProductByName(updatesToReview.productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Find the review to update.
            var reviewToUpdate = _reviewRepository.GetReviewsByUserId(userId)
                .FirstOrDefault(r => r.productId == product.productId);

            if (reviewToUpdate == null)
            {
                throw new InvalidOperationException("User has not reviewed this product");
            }

            // Update the review details.
            reviewToUpdate.reviewDate = DateTime.Now;
            reviewToUpdate.rating = updatesToReview.rating;
            reviewToUpdate.comment = updatesToReview.comment;

            // Save the updated review and recalculate the average rating for the product.
            int updatedReviewId = _reviewRepository.UpdateReview(reviewToUpdate);

            double newAvgRating = _reviewRepository.GetProductReviews(product.productId)
                .Average(r => r.rating);

            // Update the product's rating with the new average.
            _productService.UpdateProductRating(product, (decimal)newAvgRating);

            return updatedReviewId;
        }

        /// <summary>
        /// Deletes a review for a product by a user.
        /// </summary>
        /// <param name="productName">The name of the product whose review is to be deleted.</param>
        /// <param name="userId">The user ID who is deleting the review.</param>
        /// <returns>The ID of the deleted review.</returns>
        public int DeleteReview(string productName, int userId)
        {
            // Validate that the product name is provided.
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentNullException("Product name is required");
            }

            // Get the product by name.
            var product = _productService.GetProductByName(productName);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check if the user exists.
            var user = _userService.GetUserByIdWithRelatedData(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Find the review to delete.
            var existingReview = _reviewRepository.GetReviewsByUserId(userId)
                .FirstOrDefault(r => r.productId == product.productId);

            if (existingReview == null)
            {
                throw new InvalidOperationException("Review not found");
            }

            // Delete the review and get the review ID.
            int deletedReviewId = _reviewRepository.DeleteReview(existingReview);

            // Recalculate the product's average rating after deleting the review.
            double? newAvgProductRating;

            if (_reviewRepository.GetProductReviews(product.productId).Any())
            {
                newAvgProductRating = _reviewRepository.GetProductReviews(product.productId).Average(r => r.rating);
            }
            else
            {
                newAvgProductRating = null; // If no reviews, set average to null.
            }

            // Update the product's rating with the new average.
            _productService.UpdateProductRating(product, (decimal?)newAvgProductRating);

            return deletedReviewId;
        }
    }
}
