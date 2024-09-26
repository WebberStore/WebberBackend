namespace Webber.Application.Exceptions;

/// <summary>
/// Custom exception for API errors.
/// </summary>
/// <param name="message"></param>
/// <param name="innerException"></param>

public class PaymentException(string message, Exception? innerException = null) : ApiException($"Payment error: {message}. exception: {innerException}", 500)
{
    
}