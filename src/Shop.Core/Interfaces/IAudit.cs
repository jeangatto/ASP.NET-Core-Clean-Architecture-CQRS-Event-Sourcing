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
    DateTime CreatedAt { get; }

    /// <summary>
    /// Identificação do usuário da criação.
    /// </summary>
    Guid? CreatedBy { get; }

    /// <summary>
    /// Data da última alteração.
    /// </summary>
    DateTime? LastModified { get; }

    /// <summary>
    /// Identificação do usuário que alterou.
    /// </summary>
    Guid? LastModifiedBy { get; }

    /// <summary>
    /// Indica se a entidade está deletada.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Incrementado a cada alteração efetuada na entidade.
    /// </summary>
    long Version { get; }

    void SetAdded(Guid? userId, DateTime createdAt);
    void SetModified(Guid? userId, DateTime lastModified);
    void SetDeleted(Guid? userId, DateTime lastModified);
}