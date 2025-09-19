using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model.Questions;
using System.Data;

namespace ImbaBetWeb.DataAccess.Stores
{
    public class SqlQuestionStore(string connectionString) : SqlStoreBase<Question, int>(), IQuestionStore
    {
        private readonly SqlColumnDefinition columnId = new("Id", SqlDbType.Int, 0, true);
        private readonly SqlColumnDefinition columnText = new("Text", SqlDbType.NVarChar, 512, false, true);
        private readonly SqlColumnDefinition columnType = new("Type", SqlDbType.Int);
        private readonly SqlColumnDefinition columnDueDate = new("DueDate", SqlDbType.DateTime2, 256);
        private readonly SqlColumnDefinition columnChoicesRaw = new("ChoicesRaw", SqlDbType.NVarChar, 512, false, true); 
        private readonly SqlColumnDefinition columnCorrectAnswer = new("CorrectAnswer", SqlDbType.NVarChar, 256, false, true);
        private readonly SqlColumnDefinition columnPoints = new("Points", SqlDbType.Int);

        public override string TableName => "Questions";
        protected override string ConnectionString => connectionString;


        public override Dictionary<SqlColumnDefinition, object?> GetParameters(Question source)
        {
            return new Dictionary<SqlColumnDefinition, object?>
            {
                { columnId, source.Id },
                { columnText, source.Text },
                { columnType, (int)source.Type },
                { columnDueDate, source.DueDate },
                { columnChoicesRaw, source.ChoicesRaw },
                { columnCorrectAnswer, source.CorrectAnswer },
                { columnPoints, source.Points },
            };
        }

        public override Dictionary<SqlColumnDefinition, Action<Question, dynamic?>> GetPropertyMap()
        {
            return new Dictionary<SqlColumnDefinition, Action<Question, dynamic?>>()
            {
                { columnId, (player, valueToBeSet) => { player.Id = valueToBeSet; } },
                { columnText, (player, valueToBeSet) => { player.Text = valueToBeSet!; } },
                { columnType, (player, valueToBeSet) => { player.Type = (QuestionType)valueToBeSet; } },
                { columnDueDate, (player, valueToBeSet) => { player.DueDate = valueToBeSet; } },
                { columnChoicesRaw, (player, valueToBeSet) => { player.ChoicesRaw = valueToBeSet; } },
                { columnCorrectAnswer, (player, valueToBeSet) => { player.CorrectAnswer = valueToBeSet; } },
                { columnPoints, (player, valueToBeSet) => { player.Points = valueToBeSet; } }
            };
        }
    }
}
