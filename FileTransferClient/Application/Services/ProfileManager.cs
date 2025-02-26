using System.Collections.Generic;
using System.Threading.Tasks;
using FileTransferClient.Application.Contracts;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Infrastructure.Data.Profiles;

namespace FileTransferClient.Application.Services
{
    public class ProfileManager : IProfileManager
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileManager(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<List<ServerProfile>> GetAllProfilesAsync()
        {
            var profiles = await _profileRepository.GetAllProfilesAsync();
            return new List<ServerProfile>(profiles);
        }

        public async Task SaveProfileAsync(ServerProfile profile)
        {
            await _profileRepository.SaveProfileAsync(profile);
        }

        public async Task DeleteProfileAsync(int profileId)
        {
            await _profileRepository.DeleteProfileAsync(profileId);
        }
    }
}
