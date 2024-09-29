using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

/// <summary>
/// Handles operations related to coupons.
/// </summary>
/// <param name="couponService">Service for managing coupon operations.</param>

[Route("api/coupons")]
[ApiController]
public class CouponsController(ICouponService couponService) : ControllerBaseEx
{
    /// <summary>
    /// Creates a new coupon.
    /// </summary>
    /// <param name="couponDto">The details of the coupon to create.</param>
    /// <returns>
    /// Returns a Created response with the new coupon's details if creation is successful.
    /// Returns a BadRequest response if creation fails.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto couponDto)
    {
        var newCoupon = await couponService.CreateCouponAsync(couponDto);
        return newCoupon != null 
            ? CreatedAtAction(nameof(GetCouponById), new { id = newCoupon.Id }, newCoupon) 
            : BadRequest("Failed to create coupon");
    }

    /// <summary>
    /// Updates an existing coupon.
    /// </summary>
    /// <param name="id">The ID of the coupon to update.</param>
    /// <param name="couponDto">The updated coupon details.</param>
    /// <returns>
    /// Returns an OK response with the updated coupon details if the update is successful.
    /// Returns a BadRequest response if the update fails.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto couponDto)
    {
        var updatedCoupon = await couponService.UpdateCouponAsync(id, couponDto); 
        return updatedCoupon != null 
            ? Ok(updatedCoupon) 
            : BadRequest("Failed to update coupon.");
    }

    /// <summary>
    /// Deletes a coupon by its ID.
    /// </summary>
    /// <param name="id">The ID of the coupon to delete.</param>
    /// <returns>
    /// Returns a NoContent response upon successful deletion.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCoupon(int id)
    {
        await couponService.DeleteCouponAsync(id); 
        return NoContent(); 
    }
    
    /// <summary>
    /// Retrieves all active coupons.
    /// </summary>
    /// <returns>
    /// Returns an OK response with a list of active coupons.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("active")] 
    public async Task<IActionResult> GetActiveCoupons()
    {
        var coupons = await couponService.GetActiveCouponsAsync(); 
        return Ok(coupons); 
    }

    /// <summary>
    /// Retrieves all expired coupons.
    /// </summary>
    /// <returns>
    /// Returns an OK response with a list of expired coupons.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredCoupons()
    {
        var coupons = await couponService.GetExpiredCouponsAsync();
        return Ok(coupons);
    }

    /// <summary>
    /// Retrieves a coupon by its ID.
    /// </summary>
    /// <param name="id">The ID of the coupon to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the coupon details if found.
    /// Returns a NotFound response if no coupon with the specified ID exists.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCouponById(int id)
    {
        var coupon = await couponService.GetCouponByIdAsync(id);
        return coupon != null ? Ok(coupon) : NotFound();
    }
}
