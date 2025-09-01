using Application.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace ElevatorTests;

public class ElevatorControllerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ElevatorControllerApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetElevators_ReturnsElevatorList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/elevators");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var elevators = await response.Content.ReadFromJsonAsync<IReadOnlyList<Elevator>>();
        Assert.NotNull(elevators);
        Assert.NotEmpty(elevators);
    }
}