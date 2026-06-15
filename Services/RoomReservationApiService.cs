
using CampusHub.API.DTOs;
using CampusHub.Web.DTOs;

public class RoomReservationApiService
{
    private readonly HttpClient _http;

    public RoomReservationApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<RoomReservationDto>> GetAll()
        => await _http.GetFromJsonAsync<List<RoomReservationDto>>("api/reservations");

 public async Task<List<RoomReservationDto>> GetByUser(string userId)
{
    return await _http.GetFromJsonAsync<List<RoomReservationDto>>
        ($"api/reservations/user/{userId}");
}
    public async Task Create(RoomReservationCreateDto dto)
        => await _http.PostAsJsonAsync("api/reservations", dto);

    public async Task Approve(int id)
        => await _http.PutAsync($"api/reservations/approve/{id}", null);

    public async Task Reject(int id)
        => await _http.PutAsync($"api/reservations/reject/{id}", null);
}
