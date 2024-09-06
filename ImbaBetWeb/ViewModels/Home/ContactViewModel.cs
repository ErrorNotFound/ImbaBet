namespace ImbaBetWeb.ViewModels.Home
{
    public class ContactViewModel
    {
        public string? Name { get; set; }
        public string? EMail { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }

        public bool SendCopy { get; set; }
    }
}
