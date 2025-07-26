namespace CrudeSearch.Api.Data.Interfaces;

public interface IExampleRepository
{
    Task<string> GetExampleDataAsync();
}