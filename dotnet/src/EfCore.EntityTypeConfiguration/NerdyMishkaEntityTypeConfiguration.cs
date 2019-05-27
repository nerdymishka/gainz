
namespace NerdyMishka.EfCore
{
    public class NerdyMishkaEntityTypeConfiguration
    {

        internal protected string TablePrefix { get; set; }

        internal protected bool SupportsSchema  { get; set; } = true;

        internal protected bool SupportsForeignKeys { get; set; } = true;

        internal protected string Schema  { get; set; }



    }
}