using CommandsService.Models;
using System.Collections.Generic;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExists(int platformId);
        bool ExternalPlatExists(int externalPlatformId);

        // Commands
        IEnumerable<Command> GetAllCommandsForPlatform(int platformId);
        Command GetCommand (int platformId, int commandId);
        void CreateCommand(int platformId, Command command);

    }
}
