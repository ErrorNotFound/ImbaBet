namespace ImbaBetWeb.Model
{
    public interface IIdentifiable<T> where T : notnull
    {
        T Id { get; set; }
    }
}
