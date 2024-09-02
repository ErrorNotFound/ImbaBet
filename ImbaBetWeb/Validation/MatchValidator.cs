using FluentValidation;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.Validation
{
    public class MatchValidator : AbstractValidator<Match>
    {
        public MatchValidator() 
        {
            // make sure goals are in the correct range
            RuleFor(m => m.GoalsA).InclusiveBetween(0, 99).OverridePropertyName("Goal");
            RuleFor(m => m.GoalsB).InclusiveBetween(0, 99).OverridePropertyName("Goal");

            // make sure alternate text is set when team is not set
            RuleFor(m => m.AlternativeTeamAText).Must((match, text) => match.TeamATeamId == null ? !string.IsNullOrEmpty(text) : true).WithMessage("Alternative Text needs to be set if no team is selected.");
            RuleFor(m => m.AlternativeTeamBText).Must((match, text) => match.TeamBTeamId == null ? !string.IsNullOrEmpty(text) : true).WithMessage("Alternative Text needs to be set if no team is selected.");

            // make sure that matches that are over, are set in the past
            RuleFor(m => m.IsOver).Must((match, isOver) => isOver ? match.DateTime < DateTime.UtcNow : true).WithMessage("Match can only be over if date is in the past");
        }
    }
}
