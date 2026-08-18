import 'package:test/test.dart';

import 'sort_comparator.dart';

void main() {
  test('sorts by age ascending', () {
    final people = [Person('Carl', 40), Person('Ada', 25), Person('Bob', 30)];
    final sorted = sortByAgeThenName(people);
    expect(sorted.map((p) => p.name).toList(), ['Ada', 'Bob', 'Carl']);
  });

  test('breaks ties by name when ages are equal', () {
    final people = [Person('Bob', 30), Person('Ada', 30)];
    final sorted = sortByAgeThenName(people);
    expect(sorted.map((p) => p.name).toList(), ['Ada', 'Bob']);
  });

  test('does not mutate the original list', () {
    final people = [Person('Carl', 40), Person('Ada', 25)];
    sortByAgeThenName(people);
    expect(people.map((p) => p.name).toList(), ['Carl', 'Ada']);
  });
}
