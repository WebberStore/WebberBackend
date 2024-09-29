using Webber.API.Attributes;
using Webber.Application.DTOs;
using Webber.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Webber.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShippingController(IShippingService shippingService) : ControllerBaseEx
{
    /// <summary>
    /// Retrieves shipping rates based on the provided shipping rate requests.
    /// </summary>
    /// <param name="requests">A list of shipping rate requests.</param>
    /// <returns>
    /// Returns an Ok response with the list of shipping rates.
    /// </returns>
    [HttpPost("rates")]
    public async Task<IActionResult> GetShippingRates([FromBody] List<ShippingRateRequest> requests)
    {
        var rates = await shippingService.GetShippingRatesAsync(requests);
        return Ok(rates); // Return Ok response with the shipping rates
    }

    /// <summary>
    /// Creates a shipment based on the provided shipment requests.
    /// </summary>
    /// <param name="requests">A list of shipment creation requests.</param>
    /// <returns>
    /// Returns an Ok response with the details of the created shipment.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("create-shipment")]
    public async Task<IActionResult> CreateShipment([FromBody] List<CreateShipmentRequest> requests)
    {
        var shipment = await shippingService.CreateShipmentAsync(requests);
        return Ok(shipment); // Return Ok response with the created shipment details
    }

    /// <summary>
    /// Tracks a shipment using the provided tracking number.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment.</param>
    /// <returns>
    /// Returns an Ok response with the tracking information.
    /// </returns>
    [AuthorizeMiddleware(["User", "Admin", "Seller"])]
    [HttpGet("track/{trackingNumber}")]
    public async Task<IActionResult> TrackShipment(string trackingNumber)
    {
        var trackingInfo = await shippingService.TrackShipmentAsync(trackingNumber);
        return Ok(trackingInfo); // Return Ok response with the tracking information
    }

    /// <summary>
    /// Cancels a shipment using the provided tracking number.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment to cancel.</param>
    /// <returns>
    /// Returns a NoContent response if the cancellation is successful.
    /// </returns>
    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("cancel/{trackingNumber}")]
    public async Task<IActionResult> CancelShipment(string trackingNumber)
    {
        await shippingService.CancelShipmentAsync(trackingNumber);
        return NoContent(); // Return NoContent if the cancellation is successful
    }
}