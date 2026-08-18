// Exercise 013 - custom getters & setters (reference solution).

import 'dart:math';

class Circle {
  double radius;

  Circle(this.radius);

  double get area => pi * radius * radius;

  double get diameter => radius * 2;

  set diameter(double value) => radius = value / 2;
}
