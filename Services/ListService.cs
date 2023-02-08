using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services.Interfaces;

namespace ToDoList.Services
{
    public class ListService : ITdListService
    {

        private readonly ApplicationDbContext _context;


        public ListService(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task AddtdItemToAccessoryAsync(int accessoryId, int tdItemId)
        {
            throw new NotImplementedException();
        }

        public async Task AddtdItemToAccessoriesAsync(IEnumerable<int> accessoryIds, int tdItemId)
        {
            try
            {

                ToDoItem? toDoItem = await _context.ToDoItems.Include(t => t.Accessories).FirstOrDefaultAsync(t => t.Id == tdItemId);
                                                   


                foreach (int accessoryId in accessoryIds)
                {
                    Accessory? accessory = await _context.Accessories.FindAsync(accessoryId);

                    if (toDoItem != null && accessory != null)
                    {
                        toDoItem.Accessories.Add(accessory);
                    }
                }

                await _context.SaveChangesAsync();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<IEnumerable<Accessory>> GetAppUserAccessoriesAsync(string appUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IstdItemInAccessory(int accessoryId, int tdItemId)
        {
            ToDoItem? toDoItem = await _context.ToDoItems.Include(t => t.Accessories).FirstOrDefaultAsync(t => t.Id == tdItemId);


            bool inAccessory = toDoItem!.Accessories.Select(a => a.Id).Contains(accessoryId);

            return inAccessory;
        }

        public async Task RemoveAlltdItemAccessoriesAsync(int tdItemId)
        {
            ToDoItem? toDoItem = await _context.ToDoItems.Include(t => t.Accessories).FirstOrDefaultAsync(t => t.Id == tdItemId);

            toDoItem!.Accessories.Clear();
            _context.Update(toDoItem);
            await _context.AddRangeAsync();
        }
    }
}
