using System;
using System.Collections.Generic;
using FeWoLearning.Exercises.Expert;
using Xunit;

namespace FeWoLearning.Exercises.Tests.Expert;

public class Ex092_CqrsMediatorTests
{
    // --- Domain: a tiny in-memory task list, mutated only through commands. ---

    private sealed record TaskItem(int Id, string Title, bool IsCompleted);

    private sealed class TaskStore
    {
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public TaskItem Add(string title)
        {
            var task = new TaskItem(_nextId++, title, IsCompleted: false);
            _tasks.Add(task);
            return task;
        }

        public void Complete(int id)
        {
            var index = _tasks.FindIndex(t => t.Id == id);
            if (index < 0)
                throw new InvalidOperationException($"Task {id} not found.");
            _tasks[index] = _tasks[index] with { IsCompleted = true };
        }

        public IReadOnlyList<TaskItem> Snapshot() => _tasks.AsReadOnly();
    }

    private sealed record AddTaskCommand(string Title) : ICqrsCommand;

    private sealed record CompleteTaskCommand(int TaskId) : ICqrsCommand;

    private sealed record GetTaskListQuery : ICqrsQuery<IReadOnlyList<TaskItem>>;

    private sealed class AddTaskCommandHandler : ICqrsCommandHandler<AddTaskCommand>
    {
        private readonly TaskStore _store;
        public AddTaskCommandHandler(TaskStore store) => _store = store;
        public void Handle(AddTaskCommand command) => _store.Add(command.Title);
    }

    private sealed class CompleteTaskCommandHandler : ICqrsCommandHandler<CompleteTaskCommand>
    {
        private readonly TaskStore _store;
        public CompleteTaskCommandHandler(TaskStore store) => _store = store;
        public void Handle(CompleteTaskCommand command) => _store.Complete(command.TaskId);
    }

    private sealed class GetTaskListQueryHandler : ICqrsQueryHandler<GetTaskListQuery, IReadOnlyList<TaskItem>>
    {
        private readonly TaskStore _store;
        public GetTaskListQueryHandler(TaskStore store) => _store = store;
        public IReadOnlyList<TaskItem> Handle(GetTaskListQuery query) => _store.Snapshot();
    }

    private static CqrsMediator BuildMediator(TaskStore store)
    {
        var mediator = new CqrsMediator();
        mediator.RegisterCommandHandler<AddTaskCommand>(new AddTaskCommandHandler(store));
        mediator.RegisterCommandHandler<CompleteTaskCommand>(new CompleteTaskCommandHandler(store));
        mediator.RegisterQueryHandler<GetTaskListQuery, IReadOnlyList<TaskItem>>(new GetTaskListQueryHandler(store));
        return mediator;
    }

    [Fact]
    public void CommandMutatesState_QueryReturnsUpdatedProjection()
    {
        var store = new TaskStore();
        var mediator = BuildMediator(store);

        mediator.Send(new AddTaskCommand("Write CQRS exercise"));
        mediator.Send(new AddTaskCommand("Review pull request"));
        mediator.Send(new CompleteTaskCommand(1));

        var tasks = mediator.Send<IReadOnlyList<TaskItem>>(new GetTaskListQuery());

        Assert.Equal(2, tasks.Count);
        Assert.Equal(new TaskItem(1, "Write CQRS exercise", IsCompleted: true), tasks[0]);
        Assert.Equal(new TaskItem(2, "Review pull request", IsCompleted: false), tasks[1]);
    }

    [Fact]
    public void Send_WithoutRegisteredCommandHandler_Throws()
    {
        var mediator = new CqrsMediator();
        Assert.Throws<InvalidOperationException>(() => mediator.Send(new AddTaskCommand("orphaned")));
    }

    [Fact]
    public void Send_WithoutRegisteredQueryHandler_Throws()
    {
        var mediator = new CqrsMediator();
        Assert.Throws<InvalidOperationException>(() => mediator.Send<IReadOnlyList<TaskItem>>(new GetTaskListQuery()));
    }
}
