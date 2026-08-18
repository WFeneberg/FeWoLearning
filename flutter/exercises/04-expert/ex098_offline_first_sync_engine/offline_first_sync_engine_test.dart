import 'package:test/test.dart';

import 'offline_first_sync_engine.dart';

void main() {
  test('write updates the local cache immediately', () {
    final engine = SyncEngine(RemoteStore());
    engine.write('name', 'Ada', 1);
    expect(engine.read('name'), 'Ada');
  });

  test('sync pushes queued writes to the remote store', () {
    final remote = RemoteStore();
    final engine = SyncEngine(remote);
    engine.write('name', 'Ada', 1);
    engine.sync();
    expect(remote.read('name')?.value, 'Ada');
  });

  test('sync refreshes the local cache from a newer remote value', () {
    final remote = RemoteStore()..write('name', const VersionedValue('Grace', 5));
    final engine = SyncEngine(remote);
    engine.write('name', 'Ada', 1);
    engine.sync();
    expect(engine.read('name'), 'Grace');
  });

  test('a newer local write wins over an older remote value', () {
    final remote = RemoteStore()..write('name', const VersionedValue('Grace', 1));
    final engine = SyncEngine(remote);
    engine.write('name', 'Ada', 5);
    engine.sync();
    expect(remote.read('name')?.value, 'Ada');
    expect(engine.read('name'), 'Ada');
  });

  test('sync clears the pending queue', () {
    final remote = RemoteStore();
    final engine = SyncEngine(remote);
    engine.write('a', '1', 1);
    engine.sync();
    engine.write('b', '2', 2);
    engine.sync();
    expect(remote.read('a')?.value, '1');
    expect(remote.read('b')?.value, '2');
  });
}
