namespace ClassLibrary1.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName {  get; set; }

        public virtual IList<Hotel>? Hotels { get; set; } // 允許為 null
    }
}