namespace CookBook.Application.Common.Models
{
    public class Recipe
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Cuisine { get; set; }
        public string? RecipeUrl { get; set; }
        public int Duration { get; set; }
        public bool? IsEditable { get; set; }
    }
}
