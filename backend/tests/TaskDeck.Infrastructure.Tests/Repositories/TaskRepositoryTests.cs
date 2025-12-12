using Microsoft.EntityFrameworkCore;
using TaskDeck.Domain.Entities;
using TaskDeck.Domain.Enums;
using TaskDeck.Infrastructure.Persistence;
using TaskDeck.Infrastructure.Repositories;
using Xunit;

namespace TaskDeck.Infrastructure.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly TaskDeckDbContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TaskDeckDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TaskDeckDbContext(options);
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTask()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Create user first
        var user = new AppUser
        {
            Id = userId,
            Email = "test@example.com",
            DisplayName = "Test User"
        };
        _context.Users.Add(user);

        // Create project
        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            OwnerId = userId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            Status = TaskItemStatus.Todo,
            Priority = TaskPriority.Medium,
            ProjectId = projectId,
            CreatedById = userId,
            Order = 1
        };

        // Act
        var result = await _repository.CreateAsync(task);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);

        var savedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(savedTask);
    }

    [Fact]
    public async Task GetByProjectIdAsync_ShouldReturnTasksOrderedByOrder()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        var user = new AppUser
        {
            Id = userId,
            Email = "test@example.com",
            DisplayName = "Test User"
        };
        _context.Users.Add(user);

        var project = new Project
        {
            Id = projectId,
            Name = "Test Project",
            OwnerId = userId
        };
        _context.Projects.Add(project);

        var tasks = new[]
        {
            new TaskItem { Id = Guid.NewGuid(), Title = "Task 3", Order = 3, ProjectId = projectId, CreatedById = userId },
            new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", Order = 1, ProjectId = projectId, CreatedById = userId },
            new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", Order = 2, ProjectId = projectId, CreatedById = userId }
        };

        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByProjectIdAsync(projectId)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Task 1", result[0].Title);
        Assert.Equal("Task 2", result[1].Title);
        Assert.Equal("Task 3", result[2].Title);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
