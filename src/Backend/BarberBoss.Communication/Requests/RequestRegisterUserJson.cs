namespace BarberBoss.Communication.Requests;

public class RequestRegisterUserJson
{
    /// <example>Rafael Souza</example>
    public string Name { get; set; } = string.Empty;

    /// <example>rafael@barberboss.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>Mínimo de 6 caracteres. É armazenada apenas como hash.</summary>
    /// <example>barberboss123</example>
    public string Password { get; set; } = string.Empty;
}
