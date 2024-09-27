using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Exceptions;
using Webber.Application.Interfaces;
using Webber.Domain.Enums;
using Webber.Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Webber.API.Controllers;

/// <summary>
/// Manages payment processing and checkout sessions for orders.
/// </summary>
/// <param name="paymentService">Service for handling payment operations.</param>
/// <param name="orderService">Service for managing order operations.</param>
/// <param name="productService">Service for managing product operations.</param>
/// <param name="options">Stripe settings configuration.</param>

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    IOrderService orderService,
    IProductService productService,
    IOptions<StripeSettings> options) : ControllerBaseEx
{
    /// <summary>
    /// Creates a Stripe checkout session for the specified order.
    /// </summary>
    /// <param name="orderId">The ID of the order for which to create the checkout session.</param>
    /// <returns>
    /// Returns an OK response with the session ID if the session is created successfully.
    /// Returns a Forbid response if the user is not authorized to access the order.
    /// Returns a BadRequest response if the order status is not pending.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("create-checkout-session/{orderId:int}")]
    public async Task<IActionResult> CreateCheckoutSession(int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(orderId);
        if (order == null || (order.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid(); // User is not authorized to access this order
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return BadRequest("Order is not in a pending state."); // Order must be pending to create a checkout session
        }

        try
        {
            var checkoutSessionDto = new CreateCheckoutSessionDto
            {
                SuccessUrl = options.Value.SuccessUrl, // URL to redirect to on successful payment
                CancelUrl = options.Value.CancelUrl, // URL to redirect to on payment cancellation
                Currency = order.Currency, // Currency for the payment
                CartItems = (await Task.WhenAll(order.OrderItems.Select(async item =>
                {
                    var product = await productService.GetProductByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new NotFoundException($"Product with ID {item.ProductId} not found.");
                    }

                    return new CartCheckoutItemDto
                    {
                        ProductName = product.Name,
                        Price = item.PriceAtPurchase,
                        Quantity = item.Quantity
                    };
                })).ConfigureAwait(false)).ToList()
            };

            var sessionId = await paymentService.CreateCheckoutSessionAsync(checkoutSessionDto);
            order.CheckoutSessionId = sessionId; // Store the session ID in the order
            await orderService.UpdateOrderAsync(orderId, order);

            return Ok(new { SessionId = sessionId }); // Return the session ID
        }
        catch (PaymentException ex)
        {
            return StatusCode(500, ex.Message); // Handle payment-related exceptions
        }
    }

    /// <summary>
    /// Creates a payment intent for the specified order.
    /// </summary>
    /// <param name="orderId">The ID of the order for which to create the payment intent.</param>
    /// <returns>
    /// Returns an OK response with the client secret if the intent is created successfully.
    /// Returns a Forbid response if the user is not authorized to access the order.
    /// Returns a BadRequest response if the order status is not pending.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("create-payment-intent/{orderId:int}")]
    public async Task<IActionResult> CreatePaymentIntent(int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(orderId);
        if (order == null || (order.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid(); // User is not authorized to access this order
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return BadRequest("Order is not in a pending state."); // Order must be pending to create a payment intent
        }

        try
        {
            var paymentIntentDto = new CreatePaymentIntentDto
            {
                Amount = order.TotalAmount, // Total amount for the payment
                Currency = order.Currency, // Currency for the payment
                OrderId = order.Id // Associated order ID
            };

            var clientSecret = await paymentService.CreatePaymentIntentAsync(paymentIntentDto);
            order.PaymentIntentId = clientSecret; // Store the payment intent ID in the order
            await orderService.UpdateOrderAsync(orderId, order);

            return Ok(new { ClientSecret = clientSecret }); // Return the client secret for the payment
        }
        catch (PaymentException ex)
        {
            return StatusCode(500, ex.Message); // Handle payment-related exceptions
        }
    }

    /// <summary>
    /// Handles incoming Stripe webhook events.
    /// </summary>
    /// <returns>
    /// Returns an OK response if the webhook is handled successfully.
    /// Returns a BadRequest response if the webhook signature is invalid.
    /// </returns>
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            if (!await paymentService.ValidateWebhookSignatureAsync(json, Request.Headers["Stripe-Signature"]!, out var webHook))
            {
                return BadRequest("Invalid Stripe webhook signature."); // Invalid signature
            }
            
            await paymentService.HandleWebhookEventAsync(webHook!); // Process the webhook event
            return Ok(); // Successfully handled the webhook event
        }
        catch (StripeException)
        {
            return BadRequest("Invalid Stripe webhook signature."); // Handle Stripe exceptions
        }
    }
}
