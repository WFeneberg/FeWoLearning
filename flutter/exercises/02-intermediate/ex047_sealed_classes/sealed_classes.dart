// Exercise 047 - sealed classes & exhaustive switch (intermediate).
//
// Goal:   Compute the area of a Shape without an `else`/`default` branch.
// Drills: sealed classes, exhaustive `switch` expressions, object patterns.
// Passes: when area() handles every Shape subtype via pattern matching, with
//         no `default` case (the compiler enforces exhaustiveness on a
//         sealed hierarchy).

sealed class Shape {}

class Circle extends Shape {
  final double radius;
  Circle(this.radius);
}

class Rectangle extends Shape {
  final double width;
  final double height;
  Rectangle(this.width, this.height);
}

double area(Shape shape) {
  throw UnimplementedError('TODO');
}
