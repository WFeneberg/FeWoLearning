// Exercise 069 — provide/inject using Symbol keys (reference solution).
import { inject, provide, type InjectionKey } from "vue";

export interface Settings {
  theme: string;
  locale: string;
}

export const SETTINGS_KEY: InjectionKey<Settings> = Symbol("settings");

export function provideSettings(settings: Settings): void {
  provide(SETTINGS_KEY, settings);
}

export function useSettings(): Settings {
  const settings = inject(SETTINGS_KEY);
  if (settings === undefined) {
    throw new Error("useSettings() called without a matching provideSettings() ancestor");
  }
  return settings;
}
