using System.Threading.Tasks;

namespace Shop.Core.Interfaces;

public interface ISyncDataBase
{
    Task SaveAsync<TQueryModel>(TQueryModel queryModel) where TQueryModel : IQueryModel;
}