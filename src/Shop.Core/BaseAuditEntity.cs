using System;
using Shop.Core.Interfaces;

namespace Shop.Core;

public abstract class BaseAuditEntity : BaseEntity, IAudit
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public long Version { get; set; }
}