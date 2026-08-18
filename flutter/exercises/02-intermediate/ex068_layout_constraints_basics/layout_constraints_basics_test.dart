import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'layout_constraints_basics.dart';

void main() {
  testWidgets('sidebar takes exactly its fixed width', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: SidebarLayout(sidebarWidth: 200))),
    );

    final sidebarSize = tester.getSize(find.byKey(const ValueKey('sidebar')));
    expect(sidebarSize.width, 200);
  });

  testWidgets('content expands to fill the remaining width', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: SidebarLayout(sidebarWidth: 200))),
    );

    final screenWidth = tester.getSize(find.byType(Scaffold)).width;
    final contentSize = tester.getSize(find.byKey(const ValueKey('content')));

    expect(contentSize.width, screenWidth - 200);
  });

  testWidgets('a narrower sidebar leaves more room for content', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(home: Scaffold(body: SidebarLayout(sidebarWidth: 50))),
    );

    final contentSize = tester.getSize(find.byKey(const ValueKey('content')));
    final screenWidth = tester.getSize(find.byType(Scaffold)).width;

    expect(contentSize.width, screenWidth - 50);
  });
}
