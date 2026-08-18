// Exercise 017 - abstract classes (reference solution).

abstract class Shape {
  double area();

  String describe() => 'Area: ${area()}';
}

class Square extends Shape {
  final double side;

  Square(this.side);

  @override
  double area() => side * side;
}
