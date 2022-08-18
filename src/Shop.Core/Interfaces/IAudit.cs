using System;

namespace Shop.Core.Interfaces;

/// <summary>
/// Representa a auditoria de uma entidade.
/// </summary>
public interface IAudit
{
    /// <summary>
    /// Data da criação.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Identificação do usuário da criação.
    /// </summary>
    Guid? CreatedBy { get; set; }

    /// <summary>
    /// Data da última alteração.
    /// </summary>
    DateTime? LastModified { get; set; }

    /// <summary>
    /// Identificação do usuário que alterou.
    /// </summary>
    Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// Indica se a entidade está deletada.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Incrementado a cada alteração efetuada na entidade.
    /// </summary>
    long Version { get; set; }
}