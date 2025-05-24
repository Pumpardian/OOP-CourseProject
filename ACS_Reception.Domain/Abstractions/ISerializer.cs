namespace ACS_Reception.Domain.Abstractions
{
    public interface ISerializer
    {
        Task SerializeJson<T>(T obj, string fileName);
        Task<T> DeserializeJson<T>(string fileName);
        Task SerializeXml<T>(T obj, string fileName);
        Task<T> DeserializeXml<T>(string fileName);
    }
}
