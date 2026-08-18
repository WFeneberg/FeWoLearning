// Exercise 026 - List.sort & Comparator (beginner).
//
// Goal:   Sort a list of people by age ascending, breaking ties by name,
//         without mutating the input list.
// Drills: List.sort, Comparator composition, compareTo.
// Passes: when sortByAgeThenName() returns a new, correctly ordered list and
//         leaves the original list untouched.

class Person {
  final String name;
  final int age;
  Person(this.name, this.age);
}

List<Person> sortByAgeThenName(List<Person> people) {
  throw UnimplementedError('TODO');
}
