using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;

namespace ImbaBetWeb.Business
{
    public class PlayerManager(IDataStoreManager dataStore,
        IWebHostEnvironment webHostEnvironment)
    {
        private readonly IDataStoreManager _dataStoreManager = dataStore;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        public async Task<Player> GetPlayerAsync(int playerId)
        {
            var players = await _dataStoreManager.GetPlayersAsync();
            var player = players.SingleOrDefault(x => x.Id == playerId);
            if (player != null)
            {
                return player;
            }
            throw new Exception($"User with id ({playerId}) not found");
        }

        public async Task<bool> DeleteProfilePicture(int playerId)
        {
            var player = await _dataStoreManager.GetPlayerByIdAsync(playerId);

            if (player.ProfilePicturePath == null)
                return true;

            try
            {
                var fileToBeDeleted = _webHostEnvironment.WebRootPath + player.ProfilePicturePath;
                File.Delete(fileToBeDeleted);
                player.ProfilePicturePath = null;
                await _dataStoreManager.UpdatePlayersAsync([player]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SetProfilePicture(int playerId, string path)
        {
            var player = await _dataStoreManager.GetPlayerByIdAsync(playerId);

            player.ProfilePicturePath = path;
            await _dataStoreManager.UpdatePlayersAsync([player]);
            return;
        }
    }
}
