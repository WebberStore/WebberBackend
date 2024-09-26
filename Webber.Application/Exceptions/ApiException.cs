namespace Webber.Application.Exceptions;
/// <summary>
/// Custom exception for API errors.
/// </summary>
/// <param name="message"></param>
/// <param name="statusCode"></param>

public class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; private set; } = statusCode;
    public override string Message { get; } = message;
}