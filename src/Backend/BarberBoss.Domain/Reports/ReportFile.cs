namespace BarberBoss.Domain.Reports;

/// <summary>Arquivo binário pronto para download, devolvido pelos casos de uso de relatório.</summary>
public record ReportFile(byte[] Content, string FileName, string ContentType)
{
    public bool IsEmpty => Content.Length == 0;
}
