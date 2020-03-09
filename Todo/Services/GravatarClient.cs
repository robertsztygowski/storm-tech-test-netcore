using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Todo.Models.TodoItems;

namespace Todo.Services
{
    public interface IGravatarClient
    {
        Task<GravatarProfile> GetGravatarProfile(string emailAddress, string userName, CancellationToken cancellationToken);
    }

    public class GravatarClient : IGravatarClient
    {
        private readonly HttpClient _httpClient;

        public GravatarClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GravatarProfile> GetGravatarProfile(string emailAddress, string userName, CancellationToken cancellationToken)
        {
            string emailHash = GetHash(emailAddress);
            HttpResponseMessage response = await _httpClient.GetAsync($"{emailHash}.json", cancellationToken);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                    var entry = await ParseResponse(response);
                    if (entry.GravatarProfiles.Any())
                    {
                        var result = entry.GravatarProfiles.First();
                        return new GravatarProfile()
                        {
                            DisplayName = result.DisplayName,
                            DoesExist = true,
                            ThumbnailUrl = result.ThumbnailUrl
                        };
                    }
                    break;
            }

            return new GravatarProfile()
            {
                DisplayName = userName,
                DoesExist = false
            };
        }

        private async Task<GravatarEntry> ParseResponse(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            GravatarEntry gravatarEntry = JsonConvert.DeserializeObject<GravatarEntry>(json);
            return gravatarEntry;
        }

        private string GetHash(string emailAddress)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.Default.GetBytes(emailAddress.Trim().ToLowerInvariant());
                var hashBytes = md5.ComputeHash(inputBytes);

                var builder = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    builder.Append(b.ToString("X2"));
                }
                return builder.ToString().ToLowerInvariant();
            }
        }
    }
}
