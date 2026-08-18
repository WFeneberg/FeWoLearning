// Exercise 086 - persisting settings with shared_preferences (advanced).
//
// Goal:   Read and write a "dark mode" boolean setting through
//         shared_preferences, defaulting to false when unset.
// Drills: SharedPreferences, async persistence, mocked initial values.
// Passes: when isDarkMode() reflects the last value written by
//         setDarkMode(), and defaults to false before anything is written.

import 'package:shared_preferences/shared_preferences.dart';

class SettingsStore {
  static const _darkModeKey = 'darkMode';

  Future<bool> isDarkMode() {
    throw UnimplementedError('TODO');
  }

  Future<void> setDarkMode(bool value) {
    throw UnimplementedError('TODO');
  }
}
