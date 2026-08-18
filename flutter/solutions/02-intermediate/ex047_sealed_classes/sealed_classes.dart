// Exercise 047 - sealed classes & exhaustive switch (reference solution).

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

double area(Shape shape) => switch (shape) {
      Circle(radius: final r) => 3.141592653589793 * r * r,
      Rectangle(width: final w, height: final h) => w * h,
    };
