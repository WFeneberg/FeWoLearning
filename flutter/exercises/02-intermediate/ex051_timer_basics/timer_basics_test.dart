import 'package:test/test.dart';

import 'timer_basics.dart';

void main() {
  test('start increments ticks on each interval until stopped', () async {
    final ticker = Ticker();
    ticker.start(const Duration(milliseconds: 5));
    await Future<void>.delayed(const Duration(milliseconds: 27));
    ticker.stop();
    final ticksAtStop = ticker.ticks;
    expect(ticksAtStop, greaterThanOrEqualTo(3));

    await Future<void>.delayed(const Duration(milliseconds: 20));
    expect(ticker.ticks, ticksAtStop);
  });
}
