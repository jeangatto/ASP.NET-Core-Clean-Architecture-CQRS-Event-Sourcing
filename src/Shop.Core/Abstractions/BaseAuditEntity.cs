using System;
using Shop.Core.Interfaces;

namespace Shop.Core.Abstractions;

public abstract class BaseAuditEntity : BaseEntity, IAudit
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? LastModified { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public long Version { get; private set; }

    public void SetAdded(Guid? userId, DateTime createdAt)
    {
        CreatedBy = userId;
        CreatedAt = createdAt;
        Version++;
    }

    public void SetModified(Guid? userId, DateTime lastModified)
    {
        LastModifiedBy = userId;
        LastModified = lastModified;
        Version++;
    }

    public void SetDeleted(Guid? userId, DateTime lastModified)
    {
        LastModifiedBy = userId;
        LastModified = lastModified;
        IsDeleted = true;
    }
}