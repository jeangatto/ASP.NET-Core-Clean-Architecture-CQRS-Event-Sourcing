using System.ComponentModel;

namespace Shop.Domain.Enums;

/// <summary>
/// Gênero.
/// </summary>
public enum EGender
{
    /// <summary>
    /// Não informar.
    /// </summary>
    [Description("Não informar")]
    None = 0,

    /// <summary>
    /// Masculino.
    /// </summary>
    [Description("Masculino")]
    Male = 1,

    /// <summary>
    /// Feminino.
    /// </summary>
    [Description("Feminino")]
    Female = 2
}