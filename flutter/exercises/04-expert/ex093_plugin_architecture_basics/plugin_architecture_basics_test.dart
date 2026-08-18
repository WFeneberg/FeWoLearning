import 'package:test/test.dart';

import 'plugin_architecture_basics.dart';

class _EchoPlugin implements Plugin {
  _EchoPlugin(this.id);
  @override
  final String id;
  @override
  String describe() => 'echo:$id';
}

void main() {
  test('register adds a plugin discoverable by id', () {
    final registry = PluginRegistry();
    registry.register(_EchoPlugin('a'));
    expect(registry.byId('a')?.describe(), 'echo:a');
  });

  test('byId returns null for an unknown id', () {
    final registry = PluginRegistry();
    expect(registry.byId('missing'), isNull);
  });

  test('all returns every plugin in registration order', () {
    final registry = PluginRegistry()
      ..register(_EchoPlugin('a'))
      ..register(_EchoPlugin('b'));
    expect(registry.all().map((p) => p.id).toList(), ['a', 'b']);
  });

  test('register rejects a duplicate id', () {
    final registry = PluginRegistry()..register(_EchoPlugin('a'));
    expect(
      () => registry.register(_EchoPlugin('a')),
      throwsA(isA<DuplicatePluginError>()),
    );
  });
}
