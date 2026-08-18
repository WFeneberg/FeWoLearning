import 'package:test/test.dart';

import 'exception_rethrow.dart';

void main() {
  test('loadUserName returns the name for a known id', () {
    expect(loadUserName({1: 'Ada'}, '1'), 'Ada');
  });

  test('loadUserName rethrows NotFoundException unchanged for unknown ids',
      () {
    expect(() => loadUserName({1: 'Ada'}, '2'),
        throwsA(isA<NotFoundException>()));
  });

  test('loadUserName wraps a parse failure in RepositoryException', () {
    expect(() => loadUserName({1: 'Ada'}, 'not-a-number'),
        throwsA(isA<RepositoryException>()));
  });

  test('RepositoryException retains the original cause', () {
    try {
      loadUserName({1: 'Ada'}, 'nope');
      fail('expected RepositoryException');
    } on RepositoryException catch (e) {
      expect(e.cause, isA<FormatException>());
    }
  });
}
