namespace TestContainers101.Api.Infra.Storage
{
    public interface IStorage
    {
        Task<string> SaveAsync(string fileName, Stream stream);
        Task<Stream> GetAsync(string fileName);
        Task<bool> DeleteAsync(string fileName);
    }
}
