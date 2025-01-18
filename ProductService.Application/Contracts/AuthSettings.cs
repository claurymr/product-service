namespace ProductService.Application.Contracts;

/// <summary>
/// Represents the authentication settings required for the application.
/// </summary>
public record AuthSettings
{
    /// <summary>
    /// Gets the username for authentication.
    /// </summary>
    public string UserName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the password for authentication.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Gets the role associated with the user.
    /// </summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Gets the secret key used for token generation.
    /// </summary>
    public string Secret { get; init; } = string.Empty;

    /// <summary>
    /// Gets the issuer of the token.
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Gets the audience for the token.
    /// </summary>
    public string Audience { get; init; } = string.Empty;
}