import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'modular_app_shell_router.dart';

void main() {
  final router = AppRouter(
    {
      '/home': (context) => const Text('home'),
      '/settings': (context) => const Text('settings'),
    },
    notFoundBuilder: (context) => const Text('not found'),
  );

  testWidgets('navigating to a registered route builds its widget',
      (tester) async {
    await tester.pumpWidget(MaterialApp(
      onGenerateRoute: router.onGenerateRoute,
      initialRoute: '/home',
    ));

    expect(find.text('home'), findsOneWidget);
  });

  testWidgets('pushing another registered route builds it', (tester) async {
    await tester.pumpWidget(MaterialApp(
      onGenerateRoute: router.onGenerateRoute,
      initialRoute: '/home',
    ));

    final context = tester.element(find.text('home'));
    Navigator.of(context).pushNamed('/settings');
    await tester.pumpAndSettle();

    expect(find.text('settings'), findsOneWidget);
  });

  testWidgets('an unknown route falls back to notFoundBuilder',
      (tester) async {
    await tester.pumpWidget(MaterialApp(
      onGenerateRoute: router.onGenerateRoute,
      initialRoute: '/does-not-exist',
    ));

    expect(find.text('not found'), findsOneWidget);
  });

  test('onGenerateRoute returns a MaterialPageRoute carrying the settings',
      () {
    final route = router.onGenerateRoute(const RouteSettings(name: '/home'));
    expect(route, isA<MaterialPageRoute<dynamic>>());
    expect(route.settings.name, '/home');
  });
}
