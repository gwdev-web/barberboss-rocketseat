namespace BarberBoss.Exception;

/// <summary>
/// Centraliza as mensagens devolvidas ao cliente da API.
/// Mantendo tudo aqui fica trivial trocar por um .resx e habilitar i18n depois.
/// </summary>
public static class ResourceMessagesException
{
    public const string UNKNOWN_ERROR = "Erro desconhecido.";
    public const string BILLING_NOT_FOUND = "Faturamento não encontrado.";

    public const string DATE_REQUIRED = "A data do faturamento é obrigatória.";
    public const string DATE_CANNOT_BE_IN_THE_FUTURE = "A data do faturamento não pode estar no futuro.";

    public const string BARBER_NAME_REQUIRED = "O nome do barbeiro é obrigatório.";
    public const string BARBER_NAME_LENGTH = "O nome do barbeiro deve ter entre 2 e 80 caracteres.";

    public const string CLIENT_NAME_REQUIRED = "O nome do cliente é obrigatório.";
    public const string CLIENT_NAME_LENGTH = "O nome do cliente deve ter entre 2 e 120 caracteres.";

    public const string SERVICE_NAME_REQUIRED = "O nome do serviço é obrigatório.";
    public const string SERVICE_NAME_LENGTH = "O nome do serviço deve ter entre 2 e 120 caracteres.";

    public const string AMOUNT_MUST_BE_GREATER_OR_EQUAL_ZERO = "O valor deve ser maior ou igual a zero.";
    public const string CANCELLED_BILLING_MUST_HAVE_ZERO_AMOUNT = "Faturamentos cancelados devem ter valor igual a zero.";

    public const string PAYMENT_METHOD_NOT_SUPPORTED = "Forma de pagamento inválida.";
    public const string STATUS_NOT_SUPPORTED = "Status inválido.";

    public const string NOTES_MAX_LENGTH = "As observações devem ter no máximo 500 caracteres.";

    public const string INVALID_PERIOD = "A data inicial não pode ser maior que a data final.";
    public const string PAGE_NUMBER_INVALID = "O número da página deve ser maior ou igual a 1.";
    public const string PAGE_SIZE_INVALID = "O tamanho da página deve estar entre 1 e 100.";

    public const string NO_BILLINGS_FOR_THE_PERIOD = "Não há faturamentos registrados no período informado.";
}
