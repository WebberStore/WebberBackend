using Webber.API.Attributes;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

/// <summary>
/// Handles operations related to the shopping cart.
/// </summary>
/// <param name="cartService">Service for managing cart operations.</param>

[Route("api/[controller]")]
[ApiController]
public class CartController(ICartService cartService) : ControllerBaseEx
{
    /// <summary>
    /// Retrieves the current user's cart.
    /// </summary>
    /// <returns>
    /// Returns an OK response with the cart details if the cart exists.
    /// Returns a NotFound response if no cart is found for the user.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await cartService.GetCartByUserIdAsync(CurrentUserId);
        return cart != null ? Ok(cart) : NotFound();
    }

    /// <summary>
    /// Adds an item to the current user's cart.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to add.</param>
    /// <param name="quantity">The quantity of the product to add.</param>
    /// <returns>
    /// Returns a NoContent response upon successful addition of the item.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("items/{productVariantId:int}/{quantity:int}")]
    public async Task<IActionResult> AddItemToCart(int productVariantId, int quantity)
    {
        await cartService.AddItemToCartAsync(CurrentUserId, productVariantId, quantity);
        return NoContent();
    }

    /// <summary>
    /// Updates the quantity of a specific item in the user's cart.
    /// </summary>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="quantity">The new quantity for the product.</param>
    /// <returns>
    /// Returns a NoContent response upon successful update.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("items/{productId:int}/{quantity:int}")]
    public async Task<IActionResult> UpdateCartItemQuantity(int productId, int quantity)
    {
        await cartService.UpdateCartItemQuantityAsync(CurrentUserId, productId, quantity);
        return NoContent();
    }

    /// <summary>
    /// Removes a specific item from the user's cart.
    /// </summary>
    /// <param name="productId">The ID of the product to remove.</param>
    /// <returns>
    /// Returns a NoContent response upon successful removal of the item.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItemFromCart(int productId)
    {
        await cartService.RemoveItemFromCartAsync(CurrentUserId, productId);
        return NoContent();
    }

    /// <summary>
    /// Clears all items from the user's cart.
    /// </summary>
    /// <returns>
    /// Returns a NoContent response upon successful clearing of the cart.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await cartService.ClearCartAsync(CurrentUserId);
        return NoContent();
    }

    /// <summary>
    /// Applies a coupon code to the user's cart.
    /// </summary>
    /// <param name="couponCode">The coupon code to apply.</param>
    /// <returns>
    /// Returns an OK response with the updated cart if the coupon is applied successfully.
    /// Returns a NotFound response if the coupon could not be applied.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("coupons/{couponCode}")]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        var cart = await cartService.ApplyCouponAsync(CurrentUserId, couponCode);
        return cart != null ? Ok(cart) : NotFound();
    }

    /// <summary>
    /// Removes the coupon from the user's cart.
    /// </summary>
    /// <returns>
    /// Returns an OK response with the updated cart if the coupon is removed successfully.
    /// Returns a NotFound response if no coupon was found to remove.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("coupons")]
    public async Task<IActionResult> RemoveCoupon()
    {
        var cart = await cartService.RemoveCouponAsync(CurrentUserId);
        return cart != null ? Ok(cart) : NotFound();
    }
}
