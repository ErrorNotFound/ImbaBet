using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlPlayerStore(string connectionString) : SqlStoreBase<Player, int>(), IPlayerStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnMemberOfCommunityId = new("MemberOfCommunityId", SqlDbType.Int, 0, false, true);
        private readonly SqlColumnDefinition columnNamePoints = new ("Points", SqlDbType.Int);
        private readonly SqlColumnDefinition columnProfilePicturePath = new ("ProfilePicturePath", SqlDbType.NVarChar, 256, false, true);

        public override string TableName => "Players";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Player source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnMemberOfCommunityId, source.MemberOfCommunityId },
                { columnNamePoints, source.Points },
                { columnProfilePicturePath, source.ProfilePicturePath }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Player, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Player, dynamic?>>()
            {
                { columnId, (player, valueToBeSet) => { player.Id = valueToBeSet; } },
                { columnMemberOfCommunityId, (player, valueToBeSet) => { player.MemberOfCommunityId = valueToBeSet; } },
                { columnNamePoints, (player, valueToBeSet) => { player.Points = valueToBeSet; } },
                { columnProfilePicturePath, (player, valueToBeSet) => { player.ProfilePicturePath = valueToBeSet; } }
            };
        }
    }
}
