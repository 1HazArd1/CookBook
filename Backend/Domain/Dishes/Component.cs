using CookBook.Domain.Common;

namespace CookBook.Domain.Dishes
{
    public class Component : IEntity
    {
        public long ComponentId { get; set; }
        public long UserId { get; set; }
        public long RecipeId { get; set; }
        public byte StepNo { get; set; }
        public string ComponentName {  get; set; }
        public byte Status { get; set; }
        public Recipe Recipe { get; set; }
        public List<Direction> Directions { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
    }
}
