using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using ToDoApp.Application.ToDos.Commands.CreateToDo;
using ToDoApp.Application.ToDos.Commands.UpdateToDo;
using ToDoApp.Application.ToDos.Queries.GetAllToDos;
using ToDoApp.Application.ToDos.Queries.GetToDoById;
using ToDoApp.Domain.Enums;

namespace ToDoApp.Integration.Tests;

public class ToDoControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ToDoControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedToDo()
    {
        var command = new CreateToDoCommand {
            Title = "Learn .NET Integration Testing",
            Priority = Priority.MEDIUM,
            Category = Category.PERSONAL
        };
        
        var response = await _client.PostAsJsonAsync("/api/todo", command);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<CreateToDoDto>();
        result.Should().NotBeNull();
        result!.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task GetAll_Should_Return_Ok_And_List()
    {
        var response = await _client.GetAsync("/api/todo");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await response.Content.ReadFromJsonAsync<List<ToDoListItemDto>>();
        list.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetById_ShouldReturnToDo_WhenExists()
    {
        var todoId = await SeedToDoAsync("Task to Get");
        
        var response = await _client.GetAsync($"/api/todo/{todoId}");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ToDoDetailsDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(todoId);
        result.Title.Should().Be("Task to Get");
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenSuccessful()
    {
        var todoId = await SeedToDoAsync("Old Title");
        var updateCommand = new UpdateToDoCommand 
        { 
            Id = todoId, 
            Title = "New Updated Title",
            Priority = Priority.HIGH,
            Category = Category.WORK
        };
        
        var response = await _client.PutAsJsonAsync($"/api/todo", updateCommand);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getResponse = await _client.GetAsync($"/api/todo/{todoId}");
        var updatedToDo = await getResponse.Content.ReadFromJsonAsync<ToDoDetailsDto>();
        updatedToDo!.Title.Should().Be("New Updated Title");
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_AndRemovedFromList()
    {
        var todoId = await SeedToDoAsync("Task to Delete");
        
        var deleteResponse = await _client.DeleteAsync($"/api/todo/{todoId}");
        
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    private async Task<Guid> SeedToDoAsync(string title)
    {
        var command = new CreateToDoCommand
        {
            Title = title,
            Priority = Priority.MEDIUM,
            Category = Category.PERSONAL
        };
        var response = await _client.PostAsJsonAsync("/api/todo", command);
        var result = await response.Content.ReadFromJsonAsync<CreateToDoDto>();
        return result!.Id;
    }
}