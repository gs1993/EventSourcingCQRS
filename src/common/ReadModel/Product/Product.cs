using ReadModel.Persistence;

namespace ReadModel.Product
{
    public class Product : IReadEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
