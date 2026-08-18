import 'package:flutter_test/flutter_test.dart';

import 'custom_painter_basics.dart';

void main() {
  test('shouldRepaint is false for identical values', () {
    final painter = BarPainter([0.1, 0.5, 0.9]);
    final oldPainter = BarPainter([0.1, 0.5, 0.9]);
    expect(painter.shouldRepaint(oldPainter), isFalse);
  });

  test('shouldRepaint is true when a value differs', () {
    final painter = BarPainter([0.1, 0.6, 0.9]);
    final oldPainter = BarPainter([0.1, 0.5, 0.9]);
    expect(painter.shouldRepaint(oldPainter), isTrue);
  });

  test('shouldRepaint is true when lengths differ', () {
    final painter = BarPainter([0.1, 0.5]);
    final oldPainter = BarPainter([0.1, 0.5, 0.9]);
    expect(painter.shouldRepaint(oldPainter), isTrue);
  });
}
