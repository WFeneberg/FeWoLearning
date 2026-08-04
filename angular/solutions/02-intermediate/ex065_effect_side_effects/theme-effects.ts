import { EffectRef, Injectable, effect, signal, untracked } from "@angular/core";

// Exercise 065 — effect() (reference solution).

@Injectable({ providedIn: "root" })
export class ThemeStore {
  readonly theme = signal<"light" | "dark">("light");

  readonly fontSize = signal(14);

  readonly written: string[] = [];

  readonly cleanups: string[] = [];

  watchTheme(): EffectRef {
    // Reads `theme` and nothing else, so nothing else can re-run it.
    return effect(() => {
      this.written.push(`theme:${this.theme()}`);
    });
  }

  watchBoth(): EffectRef {
    // No dependency list anywhere: whatever the body reads is what it depends on.
    return effect(() => {
      this.written.push(`both:${this.theme()}/${this.fontSize()}`);
    });
  }

  watchWithCleanup(): EffectRef {
    return effect((onCleanup) => {
      const theme = this.theme();
      this.written.push(`open:${theme}`);
      // Closes over this run's value, so the cleanup reports what it is actually cleaning up
      // rather than whatever the signal happens to say later.
      onCleanup(() => {
        this.cleanups.push(`close:${theme}`);
      });
    });
  }

  watchThemeIgnoringSize(): EffectRef {
    return effect(() => {
      const theme = this.theme();
      // Current value, no subscription — so a size change alone cannot re-run this.
      const size = untracked(() => this.fontSize());
      this.written.push(`themeOnly:${theme}/${size}`);
    });
  }
}
