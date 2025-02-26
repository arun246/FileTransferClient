using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FileTransferClient.Domain.Models.Connection;
using FileTransferClient.Infrastructure.Data;

namespace FileTransferClient.Infrastructure.Data.Profiles
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _dbContext;

        public ProfileRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ServerProfile>> GetAllProfilesAsync()
        {
            var profiles = await _dbContext.ServerProfiles.ToListAsync();
            if (profiles.Count == 0)
            {
                // Add a default profile if none exist
                var defaultProfile = new ServerProfile("defaultHost", 21, "defaultUser", "defaultPassword");
                await SaveProfileAsync(defaultProfile);
                profiles.Add(defaultProfile);
            }
            return profiles;
        }

        public async Task SaveProfileAsync(ServerProfile profile)
        {
            if (profile.Id == 0)
                await _dbContext.ServerProfiles.AddAsync(profile);
            else
                _dbContext.ServerProfiles.Update(profile);

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProfileAsync(int profileId)
        {
            var profile = await _dbContext.ServerProfiles.FindAsync(profileId);
            if (profile != null)
            {
                _dbContext.ServerProfiles.Remove(profile);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
