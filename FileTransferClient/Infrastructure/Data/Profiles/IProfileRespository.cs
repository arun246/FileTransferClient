using System.Collections.Generic;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models.Connection;

namespace FileTransferClient.Infrastructure.Data.Profiles
{
    public interface IProfileRepository
    {
        Task<IEnumerable<ServerProfile>> GetAllProfilesAsync();
        Task SaveProfileAsync(ServerProfile profile);
        Task DeleteProfileAsync(int profileId);
    }
}
