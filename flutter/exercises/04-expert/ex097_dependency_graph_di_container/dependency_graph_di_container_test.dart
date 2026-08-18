import 'package:test/test.dart';

import 'dependency_graph_di_container.dart';

class Widget {
  Widget(this.id);
  final int id;
}

void main() {
  test('transient resolutions are always distinct instances', () {
    var counter = 0;
    final container = DiContainer()
      ..register<Widget>(() => Widget(counter++), lifetime: Lifetime.transient);

    final a = container.resolve<Widget>();
    final b = container.resolve<Widget>();

    expect(identical(a, b), isFalse);
  });

  test('singletons are identical across different scopes', () {
    var counter = 0;
    final container = DiContainer()
      ..register<Widget>(() => Widget(counter++), lifetime: Lifetime.singleton);

    final a = container.createScope().resolve<Widget>();
    final b = container.createScope().resolve<Widget>();

    expect(identical(a, b), isTrue);
  });

  test('scoped instances are identical within one scope', () {
    var counter = 0;
    final container = DiContainer()
      ..register<Widget>(() => Widget(counter++), lifetime: Lifetime.scoped);

    final scope = container.createScope();
    final a = scope.resolve<Widget>();
    final b = scope.resolve<Widget>();

    expect(identical(a, b), isTrue);
  });

  test('scoped instances differ across separate scopes', () {
    var counter = 0;
    final container = DiContainer()
      ..register<Widget>(() => Widget(counter++), lifetime: Lifetime.scoped);

    final a = container.createScope().resolve<Widget>();
    final b = container.createScope().resolve<Widget>();

    expect(identical(a, b), isFalse);
  });
}
