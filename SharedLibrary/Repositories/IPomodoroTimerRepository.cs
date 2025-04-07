using SharedLibrary.Models;

namespace SharedLibrary.Repositories
{
    public interface IPomodoroTimerRepository
    {
        Task<PomodoroTimer> GetById(string id);
        Task<List<PomodoroTimer>> GetByUserId(string userId);
        Task<List<PomodoroTimer>> GetAll();
        Task Add(PomodoroTimer timer);
        Task Update(PomodoroTimer timer);
        Task Delete(PomodoroTimer timer);
        Task Delete(string id);
    }
}