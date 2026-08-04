import { computed, Injectable, signal } from "@angular/core";

// Exercise 031 — immutable object updates in signals (beginner).
// Goal:   change one field of an object in a signal without mutating the object.
// Drills: update() with the spread operator, patching with a Partial, updating a *nested*
//         object, and the reference-equality rule again — this time one level down.
// Passes: when `npx jest exercises/01-beginner/ex031_signal_object_update` is green.
//
// Same rule as exercise 030, and the same trap: `settings().theme = "dark"` writes the value
// and notifies nobody, because the object reference did not change. `{...old, theme: "dark"}`
// makes a new object, so the signal sees a new reference and everything downstream updates.
//
// The extra wrinkle here is nesting. `{...old, profile: {...old.profile, name}}` has to
// rebuild *every level from the root down to the field being changed* — spreading only the
// outer object leaves `profile` pointing at the same nested object, so a computed that
// depends on the nested value still sees the old one. Anything not on that path keeps its
// original reference, which is the point: the copy is shallow and cheap.

export interface Profile {
  readonly name: string;
  readonly email: string;
}

export interface Settings {
  readonly theme: "light" | "dark";
  readonly notifications: boolean;
  readonly pageSize: number;
  readonly profile: Profile;
}

/** The reference values. Never held by the store directly — see `defaultSettings()`. */
export const DEFAULT_SETTINGS: Settings = {
  theme: "light",
  notifications: true,
  pageSize: 25,
  profile: { name: "Anonymous", email: "" },
};

/**
 * A fresh copy of the defaults, every call.
 *
 * The store must never hold DEFAULT_SETTINGS itself: `setThemeByMutating` below writes
 * straight into whatever object the signal is holding, and if that were the shared
 * constant the damage would outlive the store and leak into everything else that reads it.
 */
export function defaultSettings(): Settings {
  return { ...DEFAULT_SETTINGS, profile: { ...DEFAULT_SETTINGS.profile } };
}

@Injectable({ providedIn: "root" })
export class SettingsStore {
  readonly settings = signal<Settings>(defaultSettings());

  /** Bumped whenever the derived summary actually recomputes. */
  recomputes = 0;

  readonly summary = computed(() => {
    this.recomputes += 1;
    const { theme, pageSize, profile } = this.settings();
    return `${profile.name} · ${theme} · ${pageSize}`;
  });

  /**
   * Whether anything differs from the defaults.
   *
   * Compared by value, not identity: the store holds its own copy, so `===` against the
   * constant would never be true.
   */
  readonly isDefault = computed(() => {
    const current = this.settings();
    return (
      current.theme === DEFAULT_SETTINGS.theme &&
      current.notifications === DEFAULT_SETTINGS.notifications &&
      current.pageSize === DEFAULT_SETTINGS.pageSize &&
      current.profile.name === DEFAULT_SETTINGS.profile.name &&
      current.profile.email === DEFAULT_SETTINGS.profile.email
    );
  });

  /** Switch to the other theme. */
  toggleTheme(): void {
    throw new Error("TODO: implement toggleTheme");
  }

  /** Set the page size. Anything outside 1..100 is a RangeError. */
  setPageSize(size: number): void {
    throw new Error("TODO: implement setPageSize");
  }

  /** Apply several fields at once, leaving the rest as they were. */
  patch(changes: Partial<Omit<Settings, "profile">>): void {
    throw new Error("TODO: implement patch");
  }

  /** Change the nested profile's name. A blank name is a RangeError. */
  renameProfile(name: string): void {
    throw new Error("TODO: implement renameProfile");
  }

  /** Back to a fresh copy of the defaults, so `isDefault` reads true again. */
  reset(): void {
    throw new Error("TODO: implement reset");
  }

  /**
   * The wrong way, kept so the spec can show what goes unnoticed.
   *
   * Assign straight into the object the signal holds, without set or update. The `readonly`
   * fields make TypeScript object, which is a hint in itself — cast it away to do the deed.
   */
  setThemeByMutating(theme: "light" | "dark"): void {
    throw new Error("TODO: implement setThemeByMutating");
  }
}
