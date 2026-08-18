// Exercise 015 - enhanced enums (reference solution).

enum Planet {
  mercury(3.7),
  earth(9.8),
  jupiter(24.8);

  final double gravity;

  const Planet(this.gravity);

  double surfaceWeight(double mass) => mass * gravity;
}
