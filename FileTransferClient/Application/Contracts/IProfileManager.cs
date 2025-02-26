using System.Collections.Generic;
using System.Threading.Tasks;
using FileTransferClient.Domain.Models.Connection;

namespace FileTransferClient.Application.Contracts
{
    public interface IProfileManager
    {
        Task<List<ServerProfile>> GetAllProfilesAsync();
        Task SaveProfileAsync(ServerProfile profile);
        Task DeleteProfileAsync(int profileId);
    }
}
