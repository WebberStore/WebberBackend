namespace Webber.Application.Exceptions;

/// <summary>
/// Custom exception for API errors.
/// </summary>
/// <param name="message"></param>

public class OutOfStockException(string message) : ApiException(message, 409)
{
    
}