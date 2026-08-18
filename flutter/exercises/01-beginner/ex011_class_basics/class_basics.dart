// Exercise 011 - class basics (beginner).
//
// Goal:   Model a 2D point with a constructor that sets its fields, and a
//         method that computes the Euclidean distance to another point.
// Drills: classes, constructors, fields.
// Passes: when Point stores x/y from its constructor and distanceTo()
//         returns the straight-line distance between two points.

class Point {
  final double x;
  final double y;

  Point(this.x, this.y);

  double distanceTo(Point other) {
    throw UnimplementedError('TODO');
  }
}
