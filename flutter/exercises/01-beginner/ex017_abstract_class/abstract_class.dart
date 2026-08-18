// Exercise 017 - abstract classes (beginner).
//
// Goal:   Give every shape a shared, human-readable description that's
//         built from its own area() — implemented once on the abstract
//         base rather than per subclass.
// Drills: abstract classes, abstract methods, calling an abstract method
//         from a concrete one.
// Passes: when describe() reports the shape's area using its own area().

abstract class Shape {
  double area();

  String describe() {
    throw UnimplementedError('TODO');
  }
}

class Square extends Shape {
  final double side;

  Square(this.side);

  @override
  double area() => side * side;
}
