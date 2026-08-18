// Exercise 093 - plugin registry & discovery (reference solution).

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
    if (_plugins.any((p) => p.id == plugin.id)) {
      throw DuplicatePluginError(plugin.id);
    }
    _plugins.add(plugin);
  }

  Plugin? byId(String id) {
    for (final plugin in _plugins) {
      if (plugin.id == id) return plugin;
    }
    return null;
  }

  List<Plugin> all() => List.unmodifiable(_plugins);
}
