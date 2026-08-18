// Exercise 016 - mixins (beginner).
//
// Goal:   Give any class with a `speed` a shared fly() description, via a
//         mixin rather than duplicating the method per class.
// Drills: mixins, `with`, mixin-declared abstract members.
// Passes: when fly() reads the implementing class's speed to build its
//         description.

mixin Flyer {
  double get speed;

  String fly() {
    throw UnimplementedError('TODO');
  }
}

class Bird with Flyer {
  @override
  final double speed;

  Bird(this.speed);
}
