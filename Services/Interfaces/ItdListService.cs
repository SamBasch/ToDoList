using ToDoList.Models;

namespace ToDoList.Services.Interfaces
{
    public interface ITdListService
    {
        public Task AddtdItemToAccessoryAsync(int accessoryId, int tdItemId);

        public Task AddtdItemToAccessoriesAsync(IEnumerable<int> accessoryIds, int tdItemId);

        public Task<IEnumerable<Accessory>> GetAppUserAccessoriesAsync(string appUserId);

        public Task<bool> IstdItemInAccessory(int accessoryId, int tdItemId);

        public Task RemoveAlltdItemAccessoriesAsync(int tdItemId);
    }
}
