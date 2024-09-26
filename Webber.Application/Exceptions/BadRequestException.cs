namespace Webber.Application.Exceptions;

/// <summary>
/// Custom exception for API errors.
/// </summary>
/// <param name="message"></param>

public class BadRequestException(string message) : ApiException(message, 400);