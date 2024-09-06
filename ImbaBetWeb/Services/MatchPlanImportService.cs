using ImbaBetWeb.Data;
using ImbaBetWeb.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ImbaBetWeb.Services
{
    public class MatchPlanImportService
    {
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private const string XML_ELEMENT_NAME_TEAM = "Team";
        private const string XML_ELEMENT_NAME_MATCHGROUP = "MatchGroup";
        private const string XML_ELEMENT_NAME_MATCH = "Match";

        private const string XML_ATTRIBUTE_NAME_NAME = "Name";
        private const string XML_ATTRIBUTE_NAME_ICON = "Icon";
        private const string XML_ATTRIBUTE_NAME_GROUPRANKING = "HasGroupRanking";
        private const string XML_ATTRIBUTE_NAME_TEAM_A = "TeamA";
        private const string XML_ATTRIBUTE_NAME_TEAM_B = "TeamB";
        private const string XML_ATTRIBUTE_NAME_DATE = "DateTime";
        private const string XML_ATTRIBUTE_NAME_ALTERNATE_A = "AlternativeTeamAText";
        private const string XML_ATTRIBUTE_NAME_ALTERNATE_B = "AlternativeTeamBText";

        public MatchPlanImportService(
            ApplicationContext context,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Dictionary<string, string>> GetTemplatesAsync()
        {
            var path = _webHostEnvironment.WebRootPath + _configuration.GetSection("Configuration")["MatchPlanTemplateDirectory"];
            var files = Directory.GetFiles(path);
            var templates = new Dictionary<string, string>();

            foreach (var file in files)
            {
                var fileContent = await File.ReadAllTextAsync(file);
                templates.Add(Path.GetFileName(file), fileContent);
            }
            return templates;
        }

        public (bool isValid, List<string>? ValidationErrors) ValidateMatchPlanXmlAsync(string content)
        {
            var errors = new List<string>();
            var xsdPath = _webHostEnvironment.WebRootPath + _configuration.GetSection("Configuration")["MatchPlanXsd"];

            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (sender, args) =>
            {
                if (args.Severity == XmlSeverityType.Warning)
                {
                    errors.Add("Matching schema not found. No validation occurred");
                }
                else
                {
                    errors.Add($"Line {args.Exception.LineNumber}: \tValidation error: " + args.Message);
                }

            };
            settings.Schemas.Add(null, xsdPath);

            XmlReader reader = XmlReader.Create(new StringReader(content), settings);

            try
            {
                while (reader.Read()) ;
            }
            catch (Exception ex)
            {
                errors.Add($"{ex.Message}");
            }

            if (errors.Any())
            {
                return (false, errors);
            }
            return (true, null);
        }

        public async Task ImportAsync(string content)
        {
            var xdoc = XDocument.Parse(content);

            var teams = xdoc.Descendants(XML_ELEMENT_NAME_TEAM).Select(t => new Team()
            {
                Name = t.Attribute(XML_ATTRIBUTE_NAME_NAME)!.Value,
                FlagCountryCode = t.Attribute(XML_ATTRIBUTE_NAME_ICON)?.Value
            }).ToList();

            var matchGroups = new List<MatchGroup>();
            var allMatches = new List<Match>();

            int order = 1;
            foreach (var mg in xdoc.Descendants(XML_ELEMENT_NAME_MATCHGROUP))
            {
                var name = mg.Attribute(XML_ATTRIBUTE_NAME_NAME)?.Value ?? "Unknown MatchGroup Name";
                var groupRanking = bool.Parse(mg.Attribute(XML_ATTRIBUTE_NAME_GROUPRANKING)?.Value ?? "false");

                var matchGroup = new MatchGroup()
                {
                    Name = name,
                    HasGroupRanking = groupRanking,
                    StackRank = order++
                };

                var matches = new List<Match>();
                foreach (var m in mg.Descendants(XML_ELEMENT_NAME_MATCH))
                {
                    var match = new Match();
                    match.MatchGroup = matchGroup;

                    if (m.Attribute(XML_ATTRIBUTE_NAME_TEAM_A) != null)
                    {
                        var attributeValue = m.Attribute(XML_ATTRIBUTE_NAME_TEAM_A)!.Value;
                        // try to find the corresponding team
                        var team = teams.SingleOrDefault(x => x.Name == attributeValue);
                        if (team != null)
                        {
                            match.TeamA = team;
                        }
                        else
                        {
                            match.AlternativeTeamAText = attributeValue;
                        }
                    }

                    if (m.Attribute(XML_ATTRIBUTE_NAME_TEAM_B) != null)
                    {
                        var attributeValue = m.Attribute(XML_ATTRIBUTE_NAME_TEAM_B)!.Value;
                        // try to find the corresponding team
                        var team = teams.SingleOrDefault(x => x.Name == attributeValue);
                        if (team != null)
                        {
                            match.TeamB = team;
                        }
                        else
                        {
                            match.AlternativeTeamBText = attributeValue;
                        }
                    }

                    match.DateTime = DateTime.Parse(m.Attribute(XML_ATTRIBUTE_NAME_DATE)!.Value, CultureInfo.InvariantCulture);

                    match.AlternativeTeamAText = m.Attribute(XML_ATTRIBUTE_NAME_ALTERNATE_A)?.Value;
                    match.AlternativeTeamBText = m.Attribute(XML_ATTRIBUTE_NAME_ALTERNATE_B)?.Value;

                    matches.Add(match);
                    allMatches.Add(match);
                }
                matchGroup.Matches = matches;

                matchGroups.Add(matchGroup);
            }

            _context.Teams.AddRange(teams);
            _context.Matches.AddRange(allMatches);
            _context.MatchGroups.AddRange(matchGroups);
            await _context.SaveChangesAsync();
        }
    }
}
