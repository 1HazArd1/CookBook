using CookBook.Domain.Common;

namespace CookBook.Domain.Dishes
{
    public class Direction : IEntity
    {
        public long DirectionId { get; set; }
        public long UserId { get; set; }
        public long ComponentId { get; set; }
        public byte StepNo { get; set; }
        public string Instruction { get; set; }
        public byte Status {  get; set; }
        public Component Component { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public long ModifiedBy { get; set; }
    }
}
