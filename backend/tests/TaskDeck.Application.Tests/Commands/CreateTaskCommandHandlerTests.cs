using Moq;
using TaskDeck.Application.Commands.Tasks;
using TaskDeck.Application.Interfaces;
using TaskDeck.Domain.Entities;
using TaskDeck.Domain.Enums;
using Xunit;

namespace TaskDeck.Application.Tests.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _handler = new CreateTaskCommandHandler(
            _taskRepositoryMock.Object,
            _userRepositoryMock.Object,
            _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateTask_WhenValidCommandProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var command = new CreateTaskCommand(
            Title: "Test Task",
            Description: "Test Description",
            Priority: TaskPriority.Medium,
            DueDate: null,
            ProjectId: projectId,
            AssignedToId: null,
            CreatedById: userId);

        var user = new AppUser
        {
            Id = userId,
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);
        _taskRepositoryMock.Setup(x => x.GetMaxOrderAsync(projectId, default)).ReturnsAsync(0);
        _taskRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TaskItem>(), default))
            .ReturnsAsync((TaskItem task, CancellationToken _) => task);
        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, default)).ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(TaskItemStatus.Todo, result.Status);
        Assert.Equal(TaskPriority.Medium, result.Priority);
        Assert.Equal(projectId, result.ProjectId);
    }
}
