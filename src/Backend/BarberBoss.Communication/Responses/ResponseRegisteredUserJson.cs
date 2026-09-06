namespace BarberBoss.Communication.Responses;

public class ResponseRegisteredUserJson
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>Token JWT já autenticado, para o cliente não precisar chamar o login em seguida.</summary>
    public string Token { get; set; } = string.Empty;
}
