import 'package:test/test.dart';

import 'completer_basics.dart';

void main() {
  test('fetchAsFuture resolves with the legacy callback result', () async {
    expect(await fetchAsFuture(5), 10);
  });

  test('fetchAsFuture resolves for zero', () async {
    expect(await fetchAsFuture(0), 0);
  });
}
