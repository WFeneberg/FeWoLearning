namespace FeWoLearning.Exercises.Intermediate;

// Exercise 041 — Generic Repository (reference solution).
public interface IEntity
{
    int Id { get; }
}

public class GenericRepository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _entities = new();

    public void Add(T entity)
    {
        if (!_entities.TryAdd(entity.Id, entity))
        {
            throw new ArgumentException($"An entity with id {entity.Id} already exists.", nameof(entity));
        }
    }

    public T? GetById(int id) => _entities.TryGetValue(id, out var entity) ? entity : default;
}
