import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'render_object_basics.dart';

void main() {
  testWidgets('sizes to the requested square when unconstrained', (tester) async {
    await tester.pumpWidget(
      const Center(child: FixedSquare(size: 40)),
    );

    expect(tester.getSize(find.byType(FixedSquare)), const Size(40, 40));
  });

  testWidgets('clamps down when constraints are tighter', (tester) async {
    await tester.pumpWidget(
      const Center(
        child: SizedBox(
          width: 20,
          height: 20,
          child: FixedSquare(size: 40),
        ),
      ),
    );

    expect(tester.getSize(find.byType(FixedSquare)), const Size(20, 20));
  });
}
