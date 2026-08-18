// Exercise 086 - persisting settings with shared_preferences (reference solution).

import 'package:shared_preferences/shared_preferences.dart';

class SettingsStore {
  static const _darkModeKey = 'darkMode';

  Future<bool> isDarkMode() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getBool(_darkModeKey) ?? false;
  }

  Future<void> setDarkMode(bool value) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(_darkModeKey, value);
  }
}
