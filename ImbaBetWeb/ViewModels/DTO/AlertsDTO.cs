using System.Text.Json;

namespace ImbaBetWeb.ViewModels.DTO
{
    [Serializable]
    public class AlertsDTO
    {
        public List<string> Infos { get; set; } = new List<string>();
        public List<string> Successes { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static AlertsDTO? FromJson(string json)
        {
            return JsonSerializer.Deserialize<AlertsDTO>(json);
        }
    }
}
