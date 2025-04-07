using Microsoft.EntityFrameworkCore;
using SharedLibrary.Data;
using SharedLibrary.Models;

namespace SharedLibrary.Repositories
{
    public class PomodoroTimerRepository(
        ApplicationDbContext context
        ) : IPomodoroTimerRepository
    {
        public async Task<PomodoroTimer> GetById(string id)
            => await context.Timers.Where(x => x.Id == id).FirstAsync();

        public async Task<List<PomodoroTimer>> GetByUserId(string userId)
            => await context.Timers.Where(x => x.UserId == userId).ToListAsync();


        public async Task<List<PomodoroTimer>> GetAll() => await context.Timers.ToListAsync();

        public async Task Add(PomodoroTimer timer)
        {
            await context.Timers.AddAsync(timer);
            await context.SaveChangesAsync();
        }

        public async Task Update(PomodoroTimer timer)
        {
            context.Timers.Update(timer);
            await context.SaveChangesAsync();
        }

        public async Task Delete(PomodoroTimer timer)
        {
            context.Timers.Remove(timer);
            await context.SaveChangesAsync();
        }
        public async Task Delete(string id)
        {
            context.Timers.Remove(await context.Timers.Where(x => x.Id.Equals(id)).FirstAsync());
            await context.SaveChangesAsync();
        }

    }
}
