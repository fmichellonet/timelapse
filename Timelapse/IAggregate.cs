namespace Timelapse
{
    public interface IAggregate<TId>
    {
        TId Id { get; }
    }
}