using System.Threading;
using System.Threading.Tasks;

namespace Nexus.Api
{
    public interface IAdminGroupService
    {
        Task<ApiActionResponse<bool>> DeleteAsync(
            string[] names, 
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<bool>> DeleteAsync(
            int[] ids,
            CancellationToken cancellationToken = default(CancellationToken)); 

        Task<ApiActionResponse<GroupModel[]>> SaveAsync(
            GroupModel[] groups,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<PagedApiActionResponse<GroupModel>> ListAsync(
            int page = 1, 
            int size = 20,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<GroupModel>> FindAsync(
            int id,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<GroupModel>> Find(
            string name,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<bool>> AddUsers(
            int id, 
            int[] userIds,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ApiActionResponse<bool>> AddUsers(
            string name, string[] usernames,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class GroupModel
    {
        public int? Id {get; set;}

        public string Name { get; set;}

        public string DisplayName { get; set; }

        public string Description {get; set; }
    }
}