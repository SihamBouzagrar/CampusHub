
using CampusHub.API.DTOs;

namespace CampusHub.Web.Services
{
    public class EventApiService
    {
        private readonly HttpClient _httpClient;

        public EventApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/events
        public async Task<IEnumerable<EventReadDto>> GetEventsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<EventReadDto>>("api/events")
        ?? new List<EventReadDto>();
        }
        

        // GET: api/events/{id}
        public async Task<EventReadDto?> GetEventByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<EventReadDto>($"api/events/{id}");
        }
         public async Task<EventReadDto?> GetById(int id)
        {
            return await _httpClient.GetFromJsonAsync<EventReadDto>($"api/events/{id}");
        }

        // POST: api/events
       public async Task Create(EventCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/events", dto);
            response.EnsureSuccessStatusCode();
        }

public async Task<List<EventReadDto>> GetAll(
    string? search = null,
    DateTime? date = null,
    string? category = null)
{
    var url = "api/events";
    var query = new List<string>();

    if (!string.IsNullOrEmpty(search))
        query.Add($"search={search}");

    if (date.HasValue)
        query.Add($"date={date.Value:yyyy-MM-dd}");

    if (!string.IsNullOrEmpty(category))
        query.Add($"category={category}");

    if (query.Any())
        url += "?" + string.Join("&", query);

    return await _httpClient
        .GetFromJsonAsync<List<EventReadDto>>(url)
        ?? new();
}

        // PUT: api/events/{id}
     public async Task Update(int id, EventCreateDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/events/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // DELETE: api/events/{id}
        public async Task<bool> DeleteEventAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/events/{id}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> Join(int id, string userId)
        {
            var response = await _httpClient.PostAsync(
                $"api/events/{id}/join?userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Leave(int id, string userId)
        {
            var response = await _httpClient.PostAsync(
                $"api/events/{id}/leave?userId={userId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsRegistered(int id, string userId)
        {
            var response = await _httpClient.GetAsync(
                $"api/events/{id}/is-registered?userId={userId}");
            if (!response.IsSuccessStatusCode) return false;

            var body = await response.Content.ReadAsStringAsync();
            return bool.TryParse(body, out var result) && result;
        }
         public async Task RemoveParticipant(int eventId, int participationId)
        {
            var response = await _httpClient.PostAsync(
                $"api/events/{eventId}/remove-participant/{participationId}", null);
            response.EnsureSuccessStatusCode();
        }
        public async Task<List<EventReadDto>> GetAll(string? search = null, DateTime? date = null)
{
    var url = "api/events";
    var query = new List<string>();
    if (!string.IsNullOrEmpty(search)) query.Add($"search={search}");
    if (date.HasValue) query.Add($"date={date.Value:yyyy-MM-dd}");
    if (query.Any()) url += "?" + string.Join("&", query);

    return await _httpClient.GetFromJsonAsync<List<EventReadDto>>(url) ?? new();
}
// EventApiService.cs
public async Task<bool> Delete(int id)
{
    var response = await _httpClient.DeleteAsync($"api/events/{id}");
    return response.IsSuccessStatusCode;
}
    }
}
