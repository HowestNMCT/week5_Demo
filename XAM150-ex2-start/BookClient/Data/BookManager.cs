using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookClient.Data
{
    public class BookManager
    {
        const string Url = "http://xam150.azurewebsites.net/api/books/";
        private string authorizationKey;

        private async Task<HttpClient> GetClient()
        {
            HttpClient client = new HttpClient();
            if(authorizationKey == null)
            {
                string result = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(result);
            }
            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            using (HttpClient client = await GetClient())
            {
                string json = await client.GetStringAsync(Url);
                return JsonConvert.DeserializeObject<IEnumerable<Book>>(json);
            }
        }

        public async Task<Book> AddAsync(string title, string author, string genre)
        {
            // TODO: use POST to add a book
            Book book = new Book()
            {
                ISBN = "",
                Title = title,
                Authors = new List<string>() { author },
                Genre = genre,
                PublishDate = DateTime.Now
            };

            using (HttpClient client = await GetClient())
            {
                string json = JsonConvert.SerializeObject(book);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url, content);
                json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Book>(json);
            }
        }

        public async Task UpdateAsync(Book book)
        {
            using (HttpClient client = await GetClient())
            {
                string json = JsonConvert.SerializeObject(book);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(Url + book.ISBN, content);
            }
        }

        public async Task DeleteAsync(string isbn)
        {
            using (HttpClient client = await GetClient())
            {
                var response = await client.DeleteAsync(Url + isbn);
            }
        }
    }
}

