using FluToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace FluToDo.Services
{

    public class TodoController : IRESTApi<TodoItem>
    {
        //https://developer.android.com/studio/run/emulator-networking
        static string URL = (DeviceInfo.Platform == DevicePlatform.Android) ? "http://10.0.2.2:8080/" : "http://localhost:8080/";

        private readonly HttpClient m_client;
        private readonly JsonSerializerOptions serializerOptions;

        public TodoController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            m_client = new HttpClient(handler);
            m_client.BaseAddress = new Uri(URL);
            m_client.DefaultRequestHeaders.Accept.Clear();
            m_client.Timeout = TimeSpan.FromSeconds(10);
            m_client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            m_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");

            serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public async Task<ObservableCollection<TodoItem>> GetAsync()
        {
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(GetUrl());
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ObservableCollection<TodoItem>>(res, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<TodoItem> GetItemAsync(string id)
        {
            TodoItem item = null;
            try
            {
                HttpResponseMessage response = await m_client.GetAsync(GetUrl(id));
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    item = JsonSerializer.Deserialize<TodoItem>(res, serializerOptions);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return item;
        }

        public async Task<Uri> CreateItemAsync(TodoItem item)
        {
            try
            {
                string json = JsonSerializer.Serialize<TodoItem>(item, serializerOptions);
                var response = await m_client.PostAsync(GetUrl("Create"),
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode ? response.Headers.Location : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<bool> UpdateItemAsync(TodoItem item)
        {
            try
            {
                string json = JsonSerializer.Serialize<TodoItem>(item, serializerOptions);
                HttpResponseMessage response = await m_client.PutAsync(GetUrl(item.Key),
                    new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                return response.StatusCode == HttpStatusCode.OK;                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                HttpResponseMessage response = await m_client.DeleteAsync(GetUrl(id));
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private string GetUrl(string data = "")
        {
            if (string.IsNullOrEmpty(data))
                return "api/Todo";

            return string.Format(CultureInfo.InvariantCulture, "api/Todo/{0}", data);
        }
    }
}