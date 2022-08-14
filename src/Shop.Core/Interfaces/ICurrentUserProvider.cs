using System;

namespace Shop.Core.Interfaces;

public interface ICurrentUserProvider
{
    Guid? GetCurrentUserId();
}