using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model.Questions;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlPlayerAnswerStore(string connectionString) : SqlStoreBase<PlayerAnswer, int>(), IPlayerAnswerStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnQuestionId = new("QuestionId", SqlDbType.Int);
        private readonly SqlColumnDefinition columnPlayerId = new("PlayerId", SqlDbType.Int);
        private readonly SqlColumnDefinition columnAnswer = new("Answer", SqlDbType.NVarChar, 256, false, true);

        public override string TableName => "PlayerAnswers";
        protected override string ConnectionString => connectionString;

        public override Dictionary<SqlColumnDefinition, object?> GetParameters(PlayerAnswer source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnQuestionId, source.QuestionId },
                { columnPlayerId, source.PlayerId },
                { columnAnswer, source.Answer }
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<PlayerAnswer, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<PlayerAnswer, dynamic?>>()
            {
                { columnId, (player, valueToBeSet) => { player.Id = valueToBeSet; } },
                { columnQuestionId, (player, valueToBeSet) => { player.QuestionId = valueToBeSet; } },
                { columnPlayerId, (player, valueToBeSet) => { player.PlayerId = valueToBeSet; } },
                { columnAnswer, (player, valueToBeSet) => { player.Answer = valueToBeSet!; } }
            };
        }
    }
}
