using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using Xamarin.Essentials;

namespace FluToDo
{
    class TodoController
    {
        private HttpClient m_client;
        JsonSerializerOptions serializerOptions;

        //https://developer.android.com/studio/run/emulator-networking
        static string URL = (DeviceInfo.Platform == DevicePlatform.Android) ? "http://10.0.2.2:8080/" : "http://localhost:8080/";


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

        private string GetUrl(string data = "")
        {
            if (string.IsNullOrEmpty(data))
                return "api/Todo";

            return string.Format(CultureInfo.InvariantCulture, "api/Todo/{0}", data);
        }

        public async Task<List<TodoItem>> Get()
        {
            try
            {

                HttpResponseMessage response = await m_client.GetAsync(GetUrl());
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TodoItem>>(res, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<TodoItem> GetItem(string data)
        {
            TodoItem item = null;
            HttpResponseMessage response = await m_client.GetAsync(GetUrl(data));
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                item = JsonSerializer.Deserialize<TodoItem>(res, serializerOptions);
            }
            return item;
        }
        public async Task<Uri> CreateItem(TodoItem data)
        {
            string json = JsonSerializer.Serialize<TodoItem>(data, serializerOptions);
            var response = await m_client.PostAsync(GetUrl("Create"),
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }

        public async Task<bool> UpdateItem(TodoItem data)
        {
            string json = JsonSerializer.Serialize<TodoItem>(data, serializerOptions);
            HttpResponseMessage response = await m_client.PutAsync(GetUrl(data.Key),
                new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> DeleteItem(string id)
        {
            HttpResponseMessage response = await m_client.DeleteAsync(GetUrl(id));
            return response.StatusCode;
        }        
    }
}