import 'dart:async';

import 'package:test/test.dart';

import 'stream_controller.dart';

void main() {
  test('multiple listeners each receive published events', () async {
    final bus = EventBus();
    final first = <String>[];
    final second = <String>[];
    final sub1 = bus.events.listen(first.add);
    final sub2 = bus.events.listen(second.add);

    bus.publish('a');
    bus.publish('b');
    await Future<void>.delayed(Duration.zero);

    expect(first, ['a', 'b']);
    expect(second, ['a', 'b']);

    await sub1.cancel();
    await sub2.cancel();
    await bus.close();
  });
}
