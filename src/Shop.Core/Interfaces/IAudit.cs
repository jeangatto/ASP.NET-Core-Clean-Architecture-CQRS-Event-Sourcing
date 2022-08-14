using System;

namespace Shop.Core.Interfaces;

public interface IAudit
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    Guid? LastModifiedBy { get; set; }
    bool IsDeleted { get; set; }
    long Version { get; set; }
}