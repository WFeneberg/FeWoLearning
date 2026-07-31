namespace FeWoLearning.Exercises.Intermediate;

// Exercise 041 — Generic Repository (intermediate).
// Goal:   Implement a generic in-memory repository, Repository<T> where T : IEntity,
//         with Add and GetById methods. The generic constraint ensures only types
//         exposing an Id can be stored — passing a type that does not implement
//         IEntity is a compile-time error, not a runtime one.
// Drills: generics, generic constraints (where T : IEntity), dictionaries, nullable
//         reference types.
public interface IEntity
{
    int Id { get; }
}

public class GenericRepository<T> where T : IEntity
{
    // Adds the entity to the repository. Throws ArgumentException if an entity
    // with the same Id has already been added.
    public void Add(T entity) => throw new NotImplementedException();

    // Returns the entity with the given id, or null if none is stored.
    public T? GetById(int id) => throw new NotImplementedException();
}
