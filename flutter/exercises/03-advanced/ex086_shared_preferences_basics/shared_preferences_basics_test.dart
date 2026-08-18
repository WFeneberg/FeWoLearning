import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';

import 'shared_preferences_basics.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  setUp(() {
    SharedPreferences.setMockInitialValues({});
  });

  test('defaults to false before anything is written', () async {
    final store = SettingsStore();
    expect(await store.isDarkMode(), isFalse);
  });

  test('reflects the last written value', () async {
    final store = SettingsStore();

    await store.setDarkMode(true);

    expect(await store.isDarkMode(), isTrue);
  });
}
