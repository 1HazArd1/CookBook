namespace iMocha.Talent.Analytics.Application.Interface.Persistence
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
