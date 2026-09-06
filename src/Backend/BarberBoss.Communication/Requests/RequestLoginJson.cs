namespace BarberBoss.Communication.Requests;

public class RequestLoginJson
{
    /// <example>rafael@barberboss.com</example>
    public string Email { get; set; } = string.Empty;

    /// <example>barberboss123</example>
    public string Password { get; set; } = string.Empty;
}
