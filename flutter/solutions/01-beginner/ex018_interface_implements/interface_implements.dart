// Exercise 018 - implicit interfaces (reference solution).

class Greeter {
  String greet(String name) => 'Hello, $name!';
}

class FormalGreeter implements Greeter {
  @override
  String greet(String name) => 'Good day, $name.';
}
