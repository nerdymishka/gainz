
namespace DankInventory.Data
{
    public class License : DankDynamicObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; } 

        public decimal Cost { get; set; }
    }
}