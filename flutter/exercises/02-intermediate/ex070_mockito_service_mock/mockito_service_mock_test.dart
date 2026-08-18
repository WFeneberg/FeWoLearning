import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';

import 'mockito_service_mock.dart';

class MockGreetingService extends Mock implements GreetingService {
  @override
  Future<String> fetchGreeting(String? name) =>
      super.noSuchMethod(
        Invocation.method(#fetchGreeting, [name]),
        returnValue: Future.value(''),
        returnValueForMissingStub: Future.value(''),
      ) as Future<String>;
}

void main() {
  testWidgets('shows a progress indicator before the mock resolves', (tester) async {
    final service = MockGreetingService();
    when(service.fetchGreeting('Ada')).thenAnswer((_) async => 'Hello, Ada!');

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: GreetingLoader(service: service, name: 'Ada'))),
    );

    expect(find.byType(CircularProgressIndicator), findsOneWidget);
  });

  testWidgets('shows the mocked greeting once the future resolves', (tester) async {
    final service = MockGreetingService();
    when(service.fetchGreeting('Ada')).thenAnswer((_) async => 'Hello, Ada!');

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: GreetingLoader(service: service, name: 'Ada'))),
    );

    await tester.pumpAndSettle();

    expect(find.text('Hello, Ada!'), findsOneWidget);
    verify(service.fetchGreeting('Ada')).called(1);
  });
}
