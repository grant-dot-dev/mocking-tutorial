using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute;
using Service;
using WebApi;
using Xunit;

namespace Test;

public class MoqApiTests
{
    private readonly Mock<INotificationService> _moqNotificationService;
    private readonly Mock<ITodoService> _moqTodoService;

    public MoqApiTests()
    {
        _moqTodoService = new Mock<ITodoService>();
        _moqNotificationService = new Mock<INotificationService>();
    }

    #region Get

    [Fact]
    public void GetAll_ReturnsExpectedData()
    {
        // Arrange
        var expectedTodos = new List<Todo>
        {
            new Todo { Id = 1, Description = "Task 1", IsCompleted = false },
            new Todo { Id = 2, Description = "Task 2", IsCompleted = true }
        };

        _moqTodoService.Setup(s => s.GetAllTodos()).Returns(expectedTodos);

       
        var sut = new TodoController(_moqTodoService.Object, _moqNotificationService.Object);

        // Act
        var response = sut.GetAllTodoItems();

        // cast necessary to access relevant properties
        var okObjectResult = (OkObjectResult)response;


        // Assert
        okObjectResult.Value.Should().BeEquivalentTo(expectedTodos);
        okObjectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public void GetAll_ReturnsEmptyArray_WhenNoItems()
    {
        // Arrange
        List<Todo> expectedTodos = [];

        _moqTodoService.Setup(s => s.GetAllTodos()).Returns(expectedTodos);

        var sut = new TodoController(_moqTodoService.Object,_moqNotificationService.Object);

        // Act
        var response = sut.GetAllTodoItems();

        // cast necessary to access relevant properties
        var okObjectResult = (OkObjectResult)response;


        // Assert
        okObjectResult.Value.Should().BeEquivalentTo(new List<Todo>());
        okObjectResult.StatusCode.Should().Be(200);
    }

    #endregion

    #region Delete

    [Fact]
    public void Delete_Returns500_AndErrorMessageThrown_WhenExceptionThrown()
    {
        // Arrange
        const string errorMessage = "Failed to delete item id doesn't exist";
        _moqTodoService
            .Setup(s => s.Delete(1)).Throws(new Exception(errorMessage));
                    
        // or less specific way (regardless of id arg) is:
        _moqTodoService
            .Setup(s => s.Delete(It.IsAny<int>())).Throws(new Exception(errorMessage));

        var sut = new TodoController(_moqTodoService.Object, _moqNotificationService.Object);

        // Act
        var response = sut.DeleteTodoItem(1);

        // cast necessary to access relevant properties
        var okObjectResult = (ObjectResult) response;

        // Assert
        okObjectResult.StatusCode.Should().Be(500);
        (okObjectResult.Value as ProblemDetails)?.Detail.Should().Be(errorMessage);
    }
    
    [Fact]
    public void DeleteAPI_CallsNotificationService_WithTaskId_AndUserId()
    {
        //reset the mock
        _moqTodoService.Setup(x=>x.Delete(1)).Verifiable();
        // Act
        var result = new TodoController(_moqTodoService.Object, _moqNotificationService.Object).DeleteTodoItem(1);
        
        // Assert
        _moqNotificationService.Verify(x => x.NotifyUserTaskCompleted(1,1)); // Defaults to Times.AtLeastOnce
    }

    #endregion
}
