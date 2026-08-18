import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'command_query_widget_arch.dart';

void main() {
  test('CounterCommands.increment mutates the store', () {
    final store = CounterStore();
    final commands = CounterCommands(store);
    commands.increment();
    expect(store.value, 1);
  });

  test('CounterCommands.reset sets the store back to zero', () {
    final store = CounterStore()..setValue(7);
    final commands = CounterCommands(store);
    commands.reset();
    expect(store.value, 0);
  });

  test('CounterQueries.currentValue reads without mutating', () {
    final store = CounterStore()..setValue(3);
    final queries = CounterQueries(store);
    expect(queries.currentValue(), 3);
    expect(store.value, 3);
  });

  testWidgets('CounterView re-renders after a command runs', (tester) async {
    final store = CounterStore();
    await tester.pumpWidget(MaterialApp(home: CounterView(store: store)));

    expect(find.text('0'), findsOneWidget);

    await tester.tap(find.byType(ElevatedButton));
    await tester.pump();

    expect(find.text('1'), findsOneWidget);
  });
}
