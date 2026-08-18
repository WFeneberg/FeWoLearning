// Exercise 032 - operator overloading (beginner).
//
// Goal:   Implement vector addition, subtraction, negation, and scalar
//         multiplication for a small 2D Vector2 type.
// Drills: operator overloading (+, -, unary -, *).
// Passes: when the four operators combine x/y components exactly like plain
//         arithmetic would.

class Vector2 {
  final double x;
  final double y;
  const Vector2(this.x, this.y);

  Vector2 operator +(Vector2 other) {
    throw UnimplementedError('TODO');
  }

  Vector2 operator -(Vector2 other) {
    throw UnimplementedError('TODO');
  }

  Vector2 operator -() {
    throw UnimplementedError('TODO');
  }

  Vector2 operator *(double scalar) {
    throw UnimplementedError('TODO');
  }

  @override
  String toString() => 'Vector2($x, $y)';
}
