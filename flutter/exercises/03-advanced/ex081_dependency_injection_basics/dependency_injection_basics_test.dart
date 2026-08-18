import 'package:test/test.dart';

import 'dependency_injection_basics.dart';

abstract class Logger {
  void log(String message);
}

class FakeLogger implements Logger {
  final messages = <String>[];
  @override
  void log(String message) => messages.add(message);
}

void main() {
  test('resolves the exact registered instance', () {
    final locator = ServiceLocator();
    final logger = FakeLogger();
    locator.registerSingleton<Logger>(logger);

    expect(identical(locator.get<Logger>(), logger), isTrue);
  });

  test('throws for an unregistered type', () {
    final locator = ServiceLocator();
    expect(() => locator.get<Logger>(), throwsStateError);
  });
}
