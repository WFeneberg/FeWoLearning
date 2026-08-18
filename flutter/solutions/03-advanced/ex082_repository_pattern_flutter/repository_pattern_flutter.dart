// Exercise 082 - repository pattern (reference solution).

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
  Future<User?> findById(String id) async {
    return _usersById[id];
  }
}
