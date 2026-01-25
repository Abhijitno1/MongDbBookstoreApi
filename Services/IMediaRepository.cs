using MongoDbBookstoreApi.Models;

namespace MongoDbBookstoreApi.Services
{
    public interface IMediaRepository
    {
        Task<string> UpsertAsync(MediaFile media);
        Task<MediaFile?> GetByIdAsync(string id);
        Task<IEnumerable<MediaFileDto>> GetAllMediaAsync();
        Task DeleteMediaAsync(string id);
    }
}
