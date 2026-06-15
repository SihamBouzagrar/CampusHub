using System.Net.Http.Json;
using CampusHub.API.DTOs;

namespace CampusHub.Web.Services
{
    public class ClubApiService
    {
        private readonly HttpClient _httpClient;
        public ClubApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET ALL
       public async Task<List<ClubDto>> GetAll(string? search = null)
{
    var url = "api/clubs";

    if (!string.IsNullOrEmpty(search))
        url += $"?search={search}";

    return await _httpClient.GetFromJsonAsync<List<ClubDto>>(url)
           ?? new();
}

        // GET BY ID
        public async Task<ClubDto?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<ClubDto>($"api/clubs/{id}");
        }

        // CREATE
       public async Task<bool> Create(ClubDto model)
{
    var response = await _httpClient.PostAsJsonAsync("api/clubs", model);
    
    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"API Create failed: {response.StatusCode} - {error}");
    }
    
    return true;
}

        // UPDATE
        public async Task Update(int id, ClubDto model)
        {
            await _httpClient.PutAsJsonAsync($"api/clubs/{id}", model);
        }

        // DELETE
        public async Task Delete(int id)
        {
            await _httpClient.DeleteAsync($"api/clubs/{id}");
        } 

public async Task<bool> Join(int id, string userId)
        {
            var response = await _httpClient.PostAsync(
                $"api/clubs/{id}/join?userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        // ✅ JOIN DEBUG — retourne le message complet pour debug
        public async Task<string> JoinDebug(int id, string userId)
        {
            var response = await _httpClient.PostAsync(
                $"api/clubs/{id}/join?userId={userId}", null);
            
            var body = await response.Content.ReadAsStringAsync();
            return $"Status: {response.StatusCode} | Body: '{body}'";
        }
        // ✅ IS MEMBER avec userId en query string
     public async Task<bool> Leave(int id, string userId)
        {
            var response = await _httpClient.PostAsync(
                $"api/clubs/{id}/leave?userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        // ✅ IS MEMBER
        public async Task<bool> IsMember(int id, string userId)
        {
            var response = await _httpClient.GetAsync(
                $"api/clubs/{id}/is-member?userId={userId}");

            if (!response.IsSuccessStatusCode) return false;

            var body = await response.Content.ReadAsStringAsync();
            return bool.TryParse(body, out var result) && result;
        }
    }
}