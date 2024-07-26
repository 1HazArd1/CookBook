using CookBook.Domain.Common;

namespace CookBook.Domain.Dishes
{
    public class Cuisine : IEntity
    {
        public long CuisineId { get; set; }
        public string CuisineName { get; set; }
        public string CuisineUrl { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
    }
}
