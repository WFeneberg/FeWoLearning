// Exercise 093 - plugin registry & discovery (expert).
//
// Goal:   Build a PluginRegistry that plugins register themselves into, and
//         that consumers query without knowing concrete plugin types ahead
//         of time (the "federated plugin" idea, without an actual federated
//         package split).
// Drills: abstract classes as interfaces, runtime registration, iterable
//         discovery, duplicate-id guarding.
// Passes: when register() rejects a duplicate id, byId() finds a registered
//         plugin, and all() returns every registered plugin in registration
//         order.

abstract class Plugin {
  String get id;
  String describe();
}

class DuplicatePluginError implements Exception {
  DuplicatePluginError(this.id);
  final String id;

  @override
  String toString() =>
      'DuplicatePluginError: a plugin with id "$id" is already registered';
}

class PluginRegistry {
  final List<Plugin> _plugins = [];

  void register(Plugin plugin) {
    throw UnimplementedError('TODO');
  }

  Plugin? byId(String id) {
    throw UnimplementedError('TODO');
  }

  List<Plugin> all() {
    throw UnimplementedError('TODO');
  }
}
