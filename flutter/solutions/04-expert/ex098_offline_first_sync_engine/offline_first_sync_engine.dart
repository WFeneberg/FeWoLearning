// Exercise 098 - offline-first local cache + sync queue (reference
// solution).

class VersionedValue {
  const VersionedValue(this.value, this.updatedAt);
  final String value;
  final int updatedAt;
}

class RemoteStore {
  final Map<String, VersionedValue> _data = {};

  VersionedValue? read(String key) => _data[key];

  void write(String key, VersionedValue value) => _data[key] = value;

  Iterable<String> get keys => _data.keys;
}

class SyncEngine {
  SyncEngine(this._remote);
  final RemoteStore _remote;

  final Map<String, VersionedValue> _cache = {};
  final Map<String, VersionedValue> _pending = {};

  String? read(String key) => _cache[key]?.value;

  void write(String key, String value, int updatedAt) {
    final versioned = VersionedValue(value, updatedAt);
    _cache[key] = versioned;
    _pending[key] = versioned;
  }

  void sync() {
    for (final entry in _pending.entries) {
      final remoteValue = _remote.read(entry.key);
      if (remoteValue == null || entry.value.updatedAt >= remoteValue.updatedAt) {
        _remote.write(entry.key, entry.value);
      }
    }
    _pending.clear();

    for (final key in _remote.keys) {
      _cache[key] = _remote.read(key)!;
    }
  }
}
