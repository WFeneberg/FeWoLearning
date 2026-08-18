// Exercise 082 - repository pattern (advanced).
//
// Goal:   Separate a User domain model from its data source behind a
//         repository interface, backed by an in-memory implementation.
// Drills: abstract interfaces, domain/data separation, async lookups.
// Passes: when InMemoryUserRepository.findById() returns a matching seeded
//         user, and null for an id that was never seeded.

class User {
  const User(this.id, this.name);

  final String id;
  final String name;
}

abstract class UserRepository {
  Future<User?> findById(String id);
}

class InMemoryUserRepository implements UserRepository {
  InMemoryUserRepository(List<User> seed)
      : _usersById = {for (final u in seed) u.id: u};

  final Map<String, User> _usersById;

  @override
  Future<User?> findById(String id) {
    throw UnimplementedError('TODO');
  }
}
