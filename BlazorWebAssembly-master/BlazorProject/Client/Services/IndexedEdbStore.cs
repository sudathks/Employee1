using BlazorProject.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace BlazorProject.Client.Services
{
    public class IndexedEdbStore
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime js;

        public IndexedEdbStore(HttpClient httpClient, IJSRuntime js)
        {
            this.httpClient = httpClient;
            this.js = js;
        }

        public async ValueTask<DateTime?> GetLastUpdateDateAsync()
        {
            Time time = await GetTimeAsync(0);
            //List<Time> time = (await GetAllTimeUpdate()).ToList();
            return time.LastUpdateTime;
        }

        public ValueTask SaveEmplyeeAsync(string storeName, Employee employee)
            => AddAsync(storeName, JsonConvert.SerializeObject(employee));

        public ValueTask StoreUpdateTime(string storeName, Time time)
            => AddAsync(storeName, JsonConvert.SerializeObject(time));

        public ValueTask<Department[]> GetAllDepartmentAsync()
        {
            return js.InvokeAsync<Department[]>(
                "indexedEdb.getAll", "Department");
        }

        public ValueTask<Time> GetTimeAsync(int id) 
        {
            return js.InvokeAsync<Time>(
                "indexedEdb.get", "Time", id);
        }

        public ValueTask<Employee> GetEmployeeAsync(string storeName, Guid id)
        {
            return js.InvokeAsync<Employee>(
                "indexedEdb.get", storeName, id);
        }

        public ValueTask<Employee[]> GetAllEmployeeAsync(string storeName)
        {
            return js.InvokeAsync<Employee[]>(
               "indexedEdb.getAll", storeName);
        }

        public ValueTask<Time[]> GetAllTimeUpdate() 
        {
            return js.InvokeAsync<Time[]>("indexedEdb.getAll", "Time");
        }

        public ValueTask UpdateEmployeeAsync(Guid id, Employee employee)
            => PutAsync("Employee", id, JsonConvert.SerializeObject(employee));

        public ValueTask DeleteEmployeeAsync(string storeName, Guid id)
            => DeleteAsync(storeName, id);

        public ValueTask DeleteTimeAsync(int id)
            => DeleteAsync("Time", id);

        ValueTask AddAsync<T>(string storeName, T value)
            => js.InvokeVoidAsync("indexedEdb.add", storeName, value);

        ValueTask DeleteAsync(string storeName, object key)
            => js.InvokeVoidAsync("indexedEdb.delete", storeName, key);

        ValueTask PutAsync<T>(string storeName, object key, T value)
            => js.InvokeVoidAsync("indexedEdb.put", storeName, key, value);

        ValueTask<T> GetAsync<T>(string storeName, object key)
            => js.InvokeAsync<T>("indexedEdb.get", storeName, key);

        public async Task SubscribeToNotifications(NotificationSubscription subscription)
        {
            var response = await httpClient.PutAsJsonAsync("notifications/subscribe", subscription);
            response.EnsureSuccessStatusCode();
        }
    }
}
