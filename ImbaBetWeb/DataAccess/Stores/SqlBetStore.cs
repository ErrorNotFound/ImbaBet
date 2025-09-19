using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlBetStore(string connectionString) : SqlStoreBase<Bet, int>(), IBetStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnMatchId = new("MatchId", SqlDbType.Int);
        private readonly SqlColumnDefinition columnPlayerId = new("PlayerId", SqlDbType.Int);
        private readonly SqlColumnDefinition columnGoalsA = new("GoalsA", SqlDbType.Int);
        private readonly SqlColumnDefinition columnGoalsB = new("GoalsB", SqlDbType.Int);
        private readonly SqlColumnDefinition columnPoints = new("Points", SqlDbType.Int);

        public override string TableName => "Bets";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Bet source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnMatchId, source.MatchId },
                { columnPlayerId, source.PlayerId },
                { columnGoalsA, source.GoalsA },
                { columnGoalsB, source.GoalsB },
                { columnPoints, source.Points }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Bet, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Bet, dynamic?>>()
            {
                { columnId, (bet, valueToBeSet) => { bet.Id = valueToBeSet; } },
                { columnMatchId, (bet, valueToBeSet) => { bet.MatchId = valueToBeSet; } },
                { columnPlayerId, (bet, valueToBeSet) => { bet.PlayerId = valueToBeSet; } },
                { columnGoalsA, (bet, valueToBeSet) => { bet.GoalsA = valueToBeSet; } },
                { columnGoalsB, (bet, valueToBeSet) => { bet.GoalsB = valueToBeSet; } },
                { columnPoints, (bet, valueToBeSet) => { bet.Points = valueToBeSet; } }
            };
        }
    }
}
