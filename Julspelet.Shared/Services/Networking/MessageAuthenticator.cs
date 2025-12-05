using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Julspelet.Shared.Services.Networking;

/// <summary>
/// Service for authenticating network messages using HMAC.
/// Prevents message tampering and ensures messages come from legitimate sources.
/// </summary>
public interface IMessageAuthenticator
{
    /// <summary>
    /// Signs a message with HMAC for authentication.
    /// </summary>
    string SignMessage(object message, string secretKey);

    /// <summary>
    /// Verifies a message signature.
    /// </summary>
    bool VerifySignature(object message, string signature, string secretKey);

    /// <summary>
    /// Generates a session-specific shared secret.
    /// </summary>
    string GenerateSessionSecret();
}

/// <summary>
/// Implements HMAC-based message authentication.
/// </summary>
public class MessageAuthenticator : IMessageAuthenticator
{
    public string SignMessage(object message, string secretKey)
    {
        if (message == null || string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("Message and secret key are required");

        // Serialize message to JSON
        var json = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(json);
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);

        // Compute HMAC-SHA256
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);

        // Return as base64 string
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifySignature(object message, string signature, string secretKey)
    {
        if (message == null || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secretKey))
            return false;

        try
        {
            // Compute expected signature
            var expectedSignature = SignMessage(message, secretKey);

            // Use constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(signature),
                Convert.FromBase64String(expectedSignature)
            );
        }
        catch
        {
            return false;
        }
    }

    public string GenerateSessionSecret()
    {
        // Generate a 32-byte random secret
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        return Convert.ToBase64String(randomBytes);
    }
}
