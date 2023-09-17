using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Shop.Domain.Entities.CustomerAggregate;
using Shop.Infrastructure.Data.Mappings;

namespace Shop.Infrastructure.Data.Context;

public class WriteDbContext : BaseDbContext<WriteDbContext>
{
    private static readonly byte[] EncryptionKey = { 189, 3, 80, 118, 242, 164, 9, 197, 106, 166, 122, 118, 161, 212, 106, 26, 171, 18, 178, 98, 86, 102, 196, 6, 78, 249, 4, 164, 66, 154, 218, 126 };
    private static readonly byte[] EncryptionVector = { 163, 225, 4, 33, 227, 178, 113, 128, 174, 23, 9, 144, 213, 158, 134, 48 };

    private readonly IEncryptionProvider _encryptionProvider;

    public WriteDbContext(DbContextOptions<WriteDbContext> dbOptions)
        : base(dbOptions)
    {
        _encryptionProvider = new AesProvider(EncryptionKey, EncryptionVector);
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.UseEncryption(_encryptionProvider);
    }
}
