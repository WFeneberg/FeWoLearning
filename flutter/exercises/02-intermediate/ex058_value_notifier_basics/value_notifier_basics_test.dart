import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'value_notifier_basics.dart';

void main() {
  testWidgets('TemperatureDisplay shows the initial value', (tester) async {
    final temperature = ValueNotifier<double>(20.0);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: TemperatureDisplay(temperature: temperature))),
    );

    expect(find.text('20.0°C'), findsOneWidget);
  });

  testWidgets('TemperatureDisplay rebuilds when the notifier value changes', (tester) async {
    final temperature = ValueNotifier<double>(20.0);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: TemperatureDisplay(temperature: temperature))),
    );

    temperature.value = 25.5;
    await tester.pump();

    expect(find.text('25.5°C'), findsOneWidget);
    expect(find.text('20.0°C'), findsNothing);
  });

  testWidgets('TemperatureDisplay formats to a single decimal', (tester) async {
    final temperature = ValueNotifier<double>(18.0);

    await tester.pumpWidget(
      MaterialApp(home: Scaffold(body: TemperatureDisplay(temperature: temperature))),
    );

    expect(find.text('18.0°C'), findsOneWidget);
  });
}
