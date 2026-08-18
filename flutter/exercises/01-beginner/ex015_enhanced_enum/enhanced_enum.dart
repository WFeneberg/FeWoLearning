// Exercise 015 - enhanced enums (beginner).
//
// Goal:   Give each planet its surface gravity, and compute how much a
//         given mass would weigh on that planet's surface.
// Drills: enhanced enums with fields and a `const` constructor, methods on
//         enum values.
// Passes: when surfaceWeight() returns mass * gravity for the receiving
//         planet.

enum Planet {
  mercury(3.7),
  earth(9.8),
  jupiter(24.8);

  final double gravity;

  const Planet(this.gravity);

  double surfaceWeight(double mass) {
    throw UnimplementedError('TODO');
  }
}
