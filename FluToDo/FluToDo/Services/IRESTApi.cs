using FluToDo.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FluToDo.Services
{
    public interface IRESTApi<T>
    {
        Task<ObservableCollection<TodoItem>> GetAsync();
        Task<T> GetItemAsync(string id);
        Task<Uri> CreateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}
