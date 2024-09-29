using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IReviewService reviewService) : ControllerBaseEx
{
    /// <summary>
    /// Creates a new review for a specified product.
    /// </summary>
    /// <param name="productId">The ID of the product to review.</param>
    /// <param name="reviewDto">The details of the review to create.</param>
    /// <returns>
    /// Returns a Created response with the new review if successful.
    /// Returns a BadRequest response if the review creation fails.
    /// </returns>
    [AuthorizeMiddleware(["User", "Seller", "Admin"])]
    [HttpPost("products/{productId:int}")]
    public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto reviewDto)
    {
        reviewDto.ProductId = productId; // Set the product ID for the review
        reviewDto.UserId = CurrentUserId; // Set the user ID for the review based on the current user

        var newReview = await reviewService.CreateReviewAsync(reviewDto);
        return newReview != null
            ? CreatedAtAction(nameof(GetReviewById), new { id = newReview.Id }, newReview) // Return Created response with review details
            : BadRequest("Failed to create review."); // Return BadRequest if review creation fails
    }

    /// <summary>
    /// Retrieves all reviews for a specified product.
    /// </summary>
    /// <param name="productId">The ID of the product for which to retrieve reviews.</param>
    /// <returns>
    /// Returns an Ok response with the list of reviews for the product.
    /// </returns>
    [HttpGet("products/{productId:int}")]
    public async Task<IActionResult> GetReviewsForProduct(int productId)
    {
        var reviews = await reviewService.GetReviewsByProductAsync(productId);
        return Ok(reviews); // Return Ok response with the reviews
    }

    /// <summary>
    /// Retrieves a specific review by its ID.
    /// </summary>
    /// <param name="id">The ID of the review to retrieve.</param>
    /// <returns>
    /// Returns an Ok response with the review if found.
    /// Returns a NotFound response if the review does not exist.
    /// </returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await reviewService.GetReviewByIdAsync(id);
        return review != null ? Ok(review) : NotFound(); // Return Ok or NotFound based on review existence
    }

    /// <summary>
    /// Updates an existing review by its ID.
    /// </summary>
    /// <param name="id">The ID of the review to update.</param>
    /// <param name="reviewDto">The updated review details.</param>
    /// <returns>
    /// Returns NoContent if the update is successful.
    /// Returns Forbid if the user is not authorized to update the review.
    /// Returns BadRequest if the update fails.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] CreateReviewDto reviewDto)
    {
        var review = await reviewService.GetReviewByIdAsync(id);

        // Check if the review exists and if the current user is authorized to update it
        if (review == null || (review.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid(); // Return Forbid if not authorized
        }

        return await reviewService.UpdateReviewAsync(id, reviewDto)
            ? NoContent() // Return NoContent if update is successful
            : BadRequest($"Failed to update review with ID {id}."); // Return BadRequest if update fails
    }

    /// <summary>
    /// Deletes a review by its ID.
    /// </summary>
    /// <param name="id">The ID of the review to delete.</param>
    /// <returns>
    /// Returns NoContent if the deletion is successful.
    /// Returns Forbid if the user is not authorized to delete the review.
    /// Returns BadRequest if the deletion fails.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await reviewService.GetReviewByIdAsync(id);

        // Check if the review exists and if the current user is authorized to delete it
        if (review == null || (review.UserId != CurrentUserId && CurrentUserRole != "Admin"))
            return Forbid(); // Return Forbid if not authorized

        return await reviewService.DeleteReviewAsync(id)
            ? NoContent() // Return NoContent if deletion is successful
            : BadRequest($"Failed to delete review with ID {id}."); // Return BadRequest if deletion fails
    }
}
