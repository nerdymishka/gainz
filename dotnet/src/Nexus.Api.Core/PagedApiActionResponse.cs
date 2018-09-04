
namespace Nexus.Api
{
    public class PagedApiActionResponse<T>
    {
        public T[] Results { get; set; }

        public bool Ok { get; set; }

        public string[] ErrorMessages { get; set; }

        public int Page { get; set; }

        public int? Hits { get; set; }

        public int Size {get; set; } = 20;
    }
}