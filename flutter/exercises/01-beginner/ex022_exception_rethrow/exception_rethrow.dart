// Exercise 022 - rethrow & exception hierarchies (beginner).
//
// Goal:   Look up a user's name by a raw string id: rethrow a
//         NotFoundException unchanged when the id is well-formed but
//         unknown, but wrap any other failure (e.g. a malformed id) in a
//         RepositoryException that keeps the original cause.
// Drills: rethrow, exception hierarchies, preserving the original cause.
// Passes: when NotFoundException propagates via `rethrow` untouched, and
//         other failures surface as RepositoryException with `cause` set.

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
  throw UnimplementedError('TODO');
}
