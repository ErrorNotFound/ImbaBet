using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlMatchStore(string connectionString) : SqlStoreBase<Match, int>(), IMatchStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnDateTime = new("DateTime", SqlDbType.DateTime2, 256);
        private readonly SqlColumnDefinition columnTeamATeamId = new("TeamATeamId", SqlDbType.Int, 0, false, true);
        private readonly SqlColumnDefinition columnTeamBTeamId = new("TeamBTeamId", SqlDbType.Int, 0, false, true);
        private readonly SqlColumnDefinition columnAlternativeTeamAText = new("AlternativeTeamAText", SqlDbType.NVarChar, 256, false, true);
        private readonly SqlColumnDefinition columnAlternativeTeamBText = new("AlternativeTeamBText", SqlDbType.NVarChar, 256, false, true);
        private readonly SqlColumnDefinition columnNameGoalsA = new("GoalsA", SqlDbType.Int);
        private readonly SqlColumnDefinition columnNameGoalsB = new("GoalsB", SqlDbType.Int);
        private readonly SqlColumnDefinition columnIsOver = new("IsOver", SqlDbType.Bit);
        private readonly SqlColumnDefinition columnMatchGroupId = new("MatchGroupId", SqlDbType.Int);

        public override string TableName => "Matches";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Match source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnDateTime, source.DateTime },
                { columnTeamATeamId, source.TeamATeamId },
                { columnTeamBTeamId, source.TeamBTeamId },
                { columnAlternativeTeamAText, source.AlternativeTeamAText },
                { columnAlternativeTeamBText, source.AlternativeTeamBText },
                { columnNameGoalsA, source.GoalsA },
                { columnNameGoalsB, source.GoalsB },
                { columnIsOver, source.IsOver },
                { columnMatchGroupId, source.MatchGroupId }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Match, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Match, dynamic?>>()
            {
                { columnId, (match, valueToBeSet) => { match.Id = valueToBeSet; } },
                { columnDateTime, (match, valueToBeSet) => { match.DateTime = valueToBeSet; } },
                { columnTeamATeamId, (match, valueToBeSet) => { match.TeamATeamId = valueToBeSet; } },
                { columnTeamBTeamId, (match, valueToBeSet) => { match.TeamBTeamId = valueToBeSet; } },
                { columnAlternativeTeamAText, (match, valueToBeSet) => { match.AlternativeTeamAText = valueToBeSet; } },
                { columnAlternativeTeamBText, (match, valueToBeSet) => { match.AlternativeTeamBText = valueToBeSet; } },
                { columnNameGoalsA, (match, valueToBeSet) => { match.GoalsA = valueToBeSet; } },
                { columnNameGoalsB, (match, valueToBeSet) => { match.GoalsB = valueToBeSet; } },
                { columnIsOver, (match, valueToBeSet) => { match.IsOver = valueToBeSet; } },
                { columnMatchGroupId, (match, valueToBeSet) => { match.MatchGroupId = valueToBeSet; } },
            };
        }
    }
}
