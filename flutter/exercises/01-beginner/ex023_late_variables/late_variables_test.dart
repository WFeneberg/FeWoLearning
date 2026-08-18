import 'package:test/test.dart';

import 'late_variables.dart';

void main() {
  test('accessing environment before initialize throws', () {
    final config = Configuration();
    expect(() => config.environment, throwsA(isA<Error>()));
  });

  test('initialize sets the environment', () {
    final config = Configuration();
    config.initialize('production');
    expect(config.environment, 'production');
  });

  test('Report.total is computed lazily and only once', () {
    computeCount = 0;
    final report = Report();
    expect(computeCount, 0);
    expect(report.total, 42);
    expect(computeCount, 1);
    expect(report.total, 42);
    expect(computeCount, 1);
  });
}
