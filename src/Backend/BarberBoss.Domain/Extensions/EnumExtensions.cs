using System.ComponentModel;
using System.Reflection;

namespace BarberBoss.Domain.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Devolve o texto do atributo [Description] do enum. Usado nos relatórios em PDF e Excel.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return attribute is null ? value.ToString() : attribute.Description;
    }
}
