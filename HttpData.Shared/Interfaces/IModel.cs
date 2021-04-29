namespace HttpData.Shared.Interfaces
{
    public interface IModel<TKey>
    {
        TKey Id { get; set; }
    }
}
