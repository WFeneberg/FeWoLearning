// Exercise 013 - custom getters & setters (beginner).
//
// Goal:   Model a circle by its radius, with a computed area getter, a
//         diameter getter, and a diameter setter that adjusts the radius.
// Drills: custom getters, custom setters.
// Passes: when area/diameter are derived from radius, and assigning
//         diameter updates radius accordingly.

class Circle {
  double radius;

  Circle(this.radius);

  double get area => throw UnimplementedError('TODO');

  double get diameter => throw UnimplementedError('TODO');

  set diameter(double value) => throw UnimplementedError('TODO');
}
