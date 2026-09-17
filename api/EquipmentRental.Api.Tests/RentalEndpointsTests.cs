using System.Net;
using System.Net.Http.Json;
using EquipmentRental.Api.Dtos;

namespace EquipmentRental.Api.Tests
{
    // The four routes through HTTP, against the real pipeline and the
    // seeded fleet. Each test rents a different machine so they do not
    // step on each other.
    public class RentalEndpointsTests : IClassFixture<ApiFactory>
    {
        private readonly HttpClient _client;

        public RentalEndpointsTests(ApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        private Task<HttpResponseMessage> RentAsync(int id, int days) =>
            _client.PostAsJsonAsync($"/api/equipment/{id}/rentals", new { days });

        [Fact]
        public async Task The_fleet_lists_eight_machines_with_their_photo_and_rate()
        {
            var fleet = await _client.GetFromJsonAsync<List<EquipmentDto>>("/api/equipment");

            Assert.NotNull(fleet);
            Assert.Equal(8, fleet.Count);
            Assert.All(fleet, e => Assert.StartsWith("https://images.unsplash.com/", e.ImageUrl));
            Assert.All(fleet, e => Assert.True(e.DailyRate > 0));
            Assert.Equal(new[] { "Earthmoving", "Planting", "Tillage" }, fleet.Select(e => e.Category).Distinct().Order());
        }

        [Fact]
        public async Task A_machine_nobody_rented_is_available_with_no_dates()
        {
            var machine = await _client.GetFromJsonAsync<EquipmentDto>("/api/equipment/1");

            Assert.NotNull(machine);
            Assert.True(machine.Available);
            Assert.Null(machine.RentedFrom);
            Assert.Null(machine.RentedUntil);
        }

        [Fact]
        public async Task An_unknown_machine_answers_404_everywhere()
        {
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/equipment/999")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await RentAsync(999, 2)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync("/api/equipment/999/rentals/current")).StatusCode);
        }

        [Fact]
        public async Task Renting_answers_201_with_the_priced_rental_and_makes_the_machine_unavailable()
        {
            var response = await RentAsync(2, 3);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal("/api/equipment/2", response.Headers.Location?.PathAndQuery);
            var rental = await response.Content.ReadFromJsonAsync<RentalDto>();
            Assert.NotNull(rental);
            Assert.Equal(3, rental.Days);
            Assert.Equal(1450 * 3, rental.Total);
            Assert.Equal(rental.StartsOn.AddDays(3), rental.EndsOn);

            var machine = await _client.GetFromJsonAsync<EquipmentDto>("/api/equipment/2");
            Assert.NotNull(machine);
            Assert.False(machine.Available);
            Assert.Equal(rental.StartsOn, machine.RentedFrom);
            Assert.Equal(rental.EndsOn, machine.RentedUntil);
        }

        [Fact]
        public async Task A_machine_already_out_answers_409()
        {
            await RentAsync(3, 5);

            var second = await RentAsync(3, 1);

            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(31)]
        public async Task Days_outside_1_to_30_answer_400(int days)
        {
            var response = await RentAsync(4, days);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Cancelling_frees_the_machine_and_a_second_cancel_answers_404()
        {
            await RentAsync(5, 7);

            var first = await _client.DeleteAsync("/api/equipment/5/rentals/current");
            var machine = await _client.GetFromJsonAsync<EquipmentDto>("/api/equipment/5");
            var second = await _client.DeleteAsync("/api/equipment/5/rentals/current");

            Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
            Assert.True(machine!.Available);
            Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        }

        [Fact]
        public async Task A_cancelled_machine_can_be_rented_again()
        {
            await RentAsync(6, 2);
            await _client.DeleteAsync("/api/equipment/6/rentals/current");

            var again = await RentAsync(6, 1);

            Assert.Equal(HttpStatusCode.Created, again.StatusCode);
        }

        [Fact]
        public async Task The_front_end_origin_gets_the_cors_header_and_an_unknown_one_does_not()
        {
            using var allowed = new HttpRequestMessage(HttpMethod.Get, "/api/equipment");
            allowed.Headers.Add("Origin", "http://localhost:4200");
            using var unknown = new HttpRequestMessage(HttpMethod.Get, "/api/equipment");
            unknown.Headers.Add("Origin", "https://evil.example");

            var allowedResponse = await _client.SendAsync(allowed);
            var unknownResponse = await _client.SendAsync(unknown);

            Assert.True(allowedResponse.Headers.Contains("Access-Control-Allow-Origin"));
            Assert.False(unknownResponse.Headers.Contains("Access-Control-Allow-Origin"));
        }
    }
}
