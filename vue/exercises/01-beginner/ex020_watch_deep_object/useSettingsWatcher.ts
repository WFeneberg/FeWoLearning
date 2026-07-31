// Exercise 020 — useSettingsWatcher composable (beginner).
// Goal:   watch a reactive object with `{ deep: true }` so any nested
//         mutation is detected, and count how many times it changed.
// Drills: reactive, watch, deep option, mutating nested properties in place.
import { type Ref } from "vue";

export interface Settings {
  theme: string;
  notifications: {
    email: boolean;
    sms: boolean;
  };
}

export interface SettingsWatcher {
  settings: Settings;
  changeCount: Ref<number>;
}

export function useSettingsWatcher(): SettingsWatcher {
  throw new Error("TODO: implement useSettingsWatcher");
}
