using CookBook.Domain.Common;

namespace CookBook.Domain.Dishes
{
    public class Recipe : IEntity
    {
        public long RecipeId { get; set; }
        public long UserId { get; set; }
        public string RecipeName { get; set; }
        public string Cuisine {  get; set; }
        public string RecipeUrl { get; set; }
        public byte Status { get; set; }
        public List<Component> Components { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set;}
    }
}
