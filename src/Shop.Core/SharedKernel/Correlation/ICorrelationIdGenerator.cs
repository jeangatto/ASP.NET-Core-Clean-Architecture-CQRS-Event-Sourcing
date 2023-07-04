namespace Shop.Core.SharedKernel.Correlation;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}