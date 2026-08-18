// Exercise 022 - rethrow & exception hierarchies (reference solution).

class NotFoundException implements Exception {
  final String id;
  NotFoundException(this.id);
}

class RepositoryException implements Exception {
  final String message;
  final Object cause;
  RepositoryException(this.message, this.cause);
}

int _parseId(String rawId) => int.parse(rawId);

String loadUserName(Map<int, String> users, String rawId) {
  try {
    final id = _parseId(rawId);
    final name = users[id];
    if (name == null) {
      throw NotFoundException(rawId);
    }
    return name;
  } catch (e) {
    if (e is NotFoundException) {
      rethrow;
    }
    throw RepositoryException('failed to load user $rawId', e);
  }
}
