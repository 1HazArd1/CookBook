using CookBook.Domain.Common;

namespace CookBook.Domain.Dishes
{
    public class Ingredients : IEntity
    {
        public long IngredientId { get; set; }
        public long RecipeId { get; set; }
        public long UserId { get; set; }
        public string Ingredient { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Recipe Recipe { get; set; }
        public long ModifiedBy { get; set; }
    }
}
