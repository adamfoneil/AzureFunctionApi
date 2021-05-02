using HttpData.Shared.Interfaces;

namespace Demo.Database
{
    public class Employee : IModel<int>
    {
        public int Id { get; set; }
    }
}
