using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Webber.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

/// <summary>
/// Handles operations related to customer orders.
/// </summary>
/// <param name="orderService">Service for managing order operations.</param>

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBaseEx
{
    /// <summary>
    /// Previews an order based on the provided order details.
    /// </summary>
    /// <param name="orderDto">The details of the order to preview.</param>
    /// <returns>
    /// Returns an OK response with the order preview details.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("preview")]
    public async Task<IActionResult> GetOrderPreview([FromBody] CreateOrderDto orderDto)
    {
        orderDto.UserId = CurrentUserId; // Set the current user ID

        var orderPreview = await orderService.GetOrderPreviewAsync(orderDto);
        return Ok(orderPreview);
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="orderDto">The details of the order to create.</param>
    /// <returns>
    /// Returns a Created response with the new order's details if creation is successful.
    /// Returns a BadRequest response if creation fails due to validation issues.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
    {
        orderDto.UserId = CurrentUserId; // Set the current user ID

        if (orderDto.OrderItems.Count == 0)
        {
            return BadRequest("Order must contain at least one item.");
        }

        if (string.IsNullOrEmpty(orderDto.PaymentMethod))
        {
            return BadRequest("Payment method is required.");
        }

        var createdOrder = await orderService.CreateOrderAsync(orderDto);

        return createdOrder != null
            ? CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder)
            : BadRequest("Failed to create order.");
    }

    /// <summary>
    /// Retrieves all orders with pagination.
    /// </summary>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of orders per page.</param>
    /// <returns>
    /// Returns an OK response with a list of orders.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var orders = await orderService.GetAllOrdersAsync(page, pageSize);
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves all orders placed by the current user.
    /// </summary>
    /// <returns>
    /// Returns an OK response with a list of the user's orders.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpGet("user")]
    public async Task<IActionResult> GetOrdersByUser()
    {
        var orders = await orderService.GetOrdersByUserAsync(CurrentUserId);
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to retrieve.</param>
    /// <returns>
    /// Returns an OK response with the order details if found.
    /// Returns a NotFound response if no order with the specified ID exists.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin", "Seller"])]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        return order != null ? Ok(order) : NotFound();
    }

    /// <summary>
    /// Cancels an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to cancel.</param>
    /// <returns>
    /// Returns a NoContent response if cancellation is successful.
    /// Returns a Forbid response if the user is not authorized to cancel the order.
    /// Returns a BadRequest response if cancellation fails.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        if (order == null || (order.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid(); // User not authorized to cancel this order
        }

        if (await orderService.CancelOrderAsync(id))
            return NoContent(); // Cancellation successful

        return BadRequest($"Failed to cancel order with ID {id}.");
    }

    /// <summary>
    /// Updates the status of an order.
    /// </summary>
    /// <param name="id">The ID of the order to update.</param>
    /// <param name="newStatus">The new status to set for the order.</param>
    /// <param name="notes">Optional notes regarding the status update.</param>
    /// <returns>
    /// Returns a NoContent response upon successful update.
    /// Returns a BadRequest response if the status is invalid.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] string newStatus, [FromQuery] string? notes)
    {
        if (!Enum.IsDefined(typeof(OrderStatus), newStatus) || !Enum.TryParse<OrderStatus>(newStatus, out var status))
        {
            return BadRequest($"Invalid status: {newStatus}");
        }
        
        await orderService.UpdateOrderStatusAsync(id, status,notes);
        return NoContent();
    }

    /// <summary>
    /// Deletes an order by its ID.
    /// </summary>
    /// <param name="id">The ID of the order to delete.</param>
    /// <returns>
    /// Returns a NoContent response upon successful deletion.
    /// Returns a BadRequest response if deletion fails.
    /// </returns>
    [AuthorizeMiddleware(["Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        if (await orderService.DeleteOrderAsync(id))
        {
            return NoContent(); // Deletion successful
        }

        return BadRequest($"Failed to delete order with ID {id}.");
    }
}
