// Exercise 032 - operator overloading (reference solution).

class Vector2 {
  final double x;
  final double y;
  const Vector2(this.x, this.y);

  Vector2 operator +(Vector2 other) => Vector2(x + other.x, y + other.y);

  Vector2 operator -(Vector2 other) => Vector2(x - other.x, y - other.y);

  Vector2 operator -() => Vector2(-x, -y);

  Vector2 operator *(double scalar) => Vector2(x * scalar, y * scalar);

  @override
  String toString() => 'Vector2($x, $y)';
}
