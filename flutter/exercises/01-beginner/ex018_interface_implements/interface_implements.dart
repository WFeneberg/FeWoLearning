// Exercise 018 - implicit interfaces (beginner).
//
// Goal:   Provide a more formal implementation of Greeter's interface —
//         `implements` uses only the class's signatures, not its body, so
//         FormalGreeter must supply its own greet().
// Drills: implicit interfaces, `implements` vs `extends`.
// Passes: when FormalGreeter.greet() returns its own formal phrasing rather
//         than Greeter's casual one.

class Greeter {
  String greet(String name) => 'Hello, $name!';
}

class FormalGreeter implements Greeter {
  @override
  String greet(String name) {
    throw UnimplementedError('TODO');
  }
}
