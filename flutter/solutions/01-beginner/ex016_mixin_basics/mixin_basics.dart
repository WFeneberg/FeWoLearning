// Exercise 016 - mixins (reference solution).

mixin Flyer {
  double get speed;

  String fly() => 'Flying at $speed km/h';
}

class Bird with Flyer {
  @override
  final double speed;

  Bird(this.speed);
}
