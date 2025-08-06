namespace ImbaBetWeb.Model
{
    public record NCommunity
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string OwnerId { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
