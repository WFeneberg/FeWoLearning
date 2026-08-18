// Exercise 098 - offline-first local cache + sync queue (expert).
//
// Goal:   Let callers write to a local cache while "offline", queuing every
//         write; sync() then pushes the queued writes to a simulated remote
//         store, resolving conflicts (same key changed both sides) with
//         last-write-wins by timestamp.
// Drills: local-first state, a pending-change queue, simple conflict
//         resolution.
// Passes: when write() updates the local cache immediately and enqueues the
//         change, and sync() applies queued writes to the remote store
//         (remote wins for a key only if its remote timestamp is newer),
//         then clears the queue and refreshes the local cache from remote.

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
    throw UnimplementedError('TODO');
  }

  void sync() {
    throw UnimplementedError('TODO');
  }
}
