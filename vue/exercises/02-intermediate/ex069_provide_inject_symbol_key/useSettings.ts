// Exercise 069 — provide/inject using Symbol keys (intermediate).
// Goal:   provide a value keyed by a shared `Symbol` (not a string) so the
//         key can never collide with another plugin/library's provide key,
//         and inject it in a descendant component.
// Drills: Symbol() as an InjectionKey, provide/inject, avoiding string keys.
import { inject, provide, type InjectionKey } from "vue";

export interface Settings {
  theme: string;
  locale: string;
}

// The injection key is a Symbol, not a string literal. Two calls to
// `Symbol()` are never equal, so this constant is the only value that can
// retrieve what was provided under it — a plain string key like "settings"
// could accidentally collide with a key used by some other composable.
export const SETTINGS_KEY: InjectionKey<Settings> = Symbol("settings");

// Called in an ancestor component's setup() to make `settings` available
// to every descendant that calls useSettings().
export function provideSettings(_settings: Settings): void {
  throw new Error("TODO: implement provideSettings");
}

// Called in a descendant component's setup() to read the settings that were
// provided higher up the tree. Throws if nothing was provided under
// SETTINGS_KEY, so a missing provider fails loudly instead of returning
// undefined silently.
export function useSettings(): Settings {
  throw new Error("TODO: implement useSettings");
}
