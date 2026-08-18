// Exercise 026 - List.sort & Comparator (reference solution).

class Person {
  final String name;
  final int age;
  Person(this.name, this.age);
}

List<Person> sortByAgeThenName(List<Person> people) {
  final sorted = List<Person>.from(people);
  sorted.sort((a, b) {
    final byAge = a.age.compareTo(b.age);
    if (byAge != 0) return byAge;
    return a.name.compareTo(b.name);
  });
  return sorted;
}
