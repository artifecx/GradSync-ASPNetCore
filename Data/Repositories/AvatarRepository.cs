using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class AvatarRepository : BaseRepository, IAvatarRepository
    {
        public AvatarRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<Avatar> GetAvatarByIdAsync(string id) =>
            await this.GetDbSet<Avatar>().FindAsync(id);

        public async Task<List<Avatar>> GetAllAvatarsAsync() =>
            await this.GetDbSet<Avatar>().AsNoTracking().ToListAsync();

        public async Task AddAvatarAsync(Avatar avatar)
        {
            await this.GetDbSet<Avatar>().AddAsync(avatar);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAvatarAsync(Avatar avatar)
        {
            this.GetDbSet<Avatar>().Update(avatar);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAvatarAsync(string id)
        {
            var avatar = await GetAvatarByIdAsync(id);
            if (avatar != null)
            {
                this.GetDbSet<Avatar>().Remove(avatar);
                await UnitOfWork.SaveChangesAsync();
            }
        }

        public bool HasChanges(Avatar avatar)
        {
            var entry = this.GetDbSet<Avatar>().Entry(avatar);
            return entry.Properties.Any(p => p.IsModified);
        }
    }
}
