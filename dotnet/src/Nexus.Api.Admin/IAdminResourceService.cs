
using System.Threading;
using System.Threading.Tasks;

namespace Nexus.Api
{
    public interface IAdminResourceService
    {
        Task<ApiActionResponse<ResourceModel[]>> SaveAsync(
            ResourceModel[] resources,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<PagedApiActionResponse<ResourceModel>> ListAsync(
            int page = 1, 
            int size = 20, 
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<ResourceModel>> FindAsync(
            long id,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<ResourceModel>> FindAsync(
            string uri,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<bool>> DeleteAsync(
            long[] ids,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ResourceModel
    {
        public long? Id { get; set; }

        public long? Key {get; set; }

        public string Type {get; set;}

        public string Uri { get; set; }

        public bool IsDeleted { get; set; }
    }
}