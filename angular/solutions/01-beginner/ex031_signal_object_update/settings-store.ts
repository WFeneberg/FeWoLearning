import { computed, Injectable, signal } from "@angular/core";

// Exercise 031 — immutable object updates in signals (reference solution).

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

export const DEFAULT_SETTINGS: Settings = {
  theme: "light",
  notifications: true,
  pageSize: 25,
  profile: { name: "Anonymous", email: "" },
};

// The store must hold a copy, never the constant itself: setThemeByMutating() writes into
// whatever object the signal holds, and corrupting the shared default would outlive the
// store entirely.
export function defaultSettings(): Settings {
  return { ...DEFAULT_SETTINGS, profile: { ...DEFAULT_SETTINGS.profile } };
}

@Injectable({ providedIn: "root" })
export class SettingsStore {
  readonly settings = signal<Settings>(defaultSettings());

  recomputes = 0;

  readonly summary = computed(() => {
    this.recomputes += 1;
    const { theme, pageSize, profile } = this.settings();
    return `${profile.name} · ${theme} · ${pageSize}`;
  });

  // By value, not identity — the store never holds the constant itself.
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

  toggleTheme(): void {
    this.settings.update((current) => ({
      ...current,
      theme: current.theme === "light" ? "dark" : "light",
    }));
  }

  setPageSize(size: number): void {
    if (size < 1 || size > 100) {
      throw new RangeError("pageSize must be between 1 and 100");
    }
    this.settings.update((current) => ({ ...current, pageSize: size }));
  }

  patch(changes: Partial<Omit<Settings, "profile">>): void {
    // Spreading `changes` last lets it win, and an empty object is naturally a no-op.
    this.settings.update((current) => ({ ...current, ...changes }));
  }

  renameProfile(name: string): void {
    const trimmed = name.trim();
    if (trimmed === "") {
      throw new RangeError("name must not be blank");
    }
    // Both levels are rebuilt. Spreading only the outer object would leave `profile`
    // pointing at the old one, and the change would be invisible downstream.
    this.settings.update((current) => ({
      ...current,
      profile: { ...current.profile, name: trimmed },
    }));
  }

  reset(): void {
    // A fresh copy, so a later mutation cannot reach back into the shared constant.
    this.settings.set(defaultSettings());
  }

  setThemeByMutating(theme: "light" | "dark"): void {
    // Deliberately wrong, and the cast is the tell: readonly was there to prevent this.
    (this.settings() as { theme: "light" | "dark" }).theme = theme;
  }
}
