using System.Net.Http.Json;
using CampusHub.API.DTOs;

namespace CampusHub.Web.Services
{
    public class AnnouncementApiService
    {
        private readonly HttpClient _httpClient;

        public AnnouncementApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AnnouncementReadDto>> GetAll(string? search = null)
        {
            var url = "api/announcements";

            if (!string.IsNullOrEmpty(search))
                url += $"?search={Uri.EscapeDataString(search)}";

            return await _httpClient.GetFromJsonAsync<List<AnnouncementReadDto>>(url)
                   ?? new List<AnnouncementReadDto>();
        }

        public async Task<AnnouncementReadDto?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<AnnouncementReadDto>(
                $"api/announcements/{id}");
        }

      public async Task Create(AnnouncementCreateDto dto, string userId)
{
    var response = await _httpClient.PostAsJsonAsync(
        $"api/announcements?userId={userId}", dto);

    var body = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        throw new Exception(body); // IMPORTANT pour voir vrai erreur
}
        public async Task Delete(int id)
        {
            var response = await _httpClient.DeleteAsync(
                $"api/announcements/{id}");

            response.EnsureSuccessStatusCode();
        }
    }
}