import { Injectable, InjectionToken, inject, signal } from "@angular/core";

// Exercise 100 — an i18n layer: message catalog, interpolation, runtime locale switching (expert).
// Goal:   translate UI strings from a plain data catalog rather than Angular's compile-time
//         `$localize`/i18n pipeline — this project builds once with Jest, not once per locale, so
//         the compiler-driven approach has nowhere to hook in. A catalog-driven service is also what
//         most real apps reach for anyway once they need to flip locale at RUNTIME (a user picking a
//         language from a menu) rather than only at BUILD time.
// Drills: a `Record<locale, Record<key, template>>` message catalog, `{{param}}` interpolation, a
//         `signal` for "current locale" so templates can react to a runtime switch, and a documented
//         two-level fallback: missing key falls back to another locale, missing locale is a hard
//         error (never silently mistranslated).
// Passes: when `npx jest exercises/04-expert/ex100_i18n_layer` is green.
//
// Two different kinds of "missing" are handled two different ways, deliberately:
//   1. The CURRENT locale is switched to a locale nothing registered a catalog for at all — this is
//      an app wiring bug (a language was added to a menu without a catalog), and `setLocale` rejects
//      it outright: the locale signal must not even change, so a bad switch can't leave the app
//      silently showing whatever locale it was on before under a wrong label.
//   2. The CURRENT locale IS registered, but is simply missing one specific key — translators ship
//      incrementally, so a freshly-added string in `en` legitimately has no `de` translation yet.
//      That is not a wiring bug, it's an expected, temporary gap: `translate()` falls back to
//      FALLBACK_LOCALE's copy of the same key instead of throwing. Only when the key is missing in
//      BOTH the current locale and the fallback does `translate()` finally throw — at that point
//      there is truly nothing left to show the user.
//
// Interpolation is deliberately strict, not permissive: a template referencing `{{name}}` with no
// `name` in the params object throws rather than leaving a literal `{{name}}` in the rendered
// string — a silently-broken placeholder in production is a worse failure than a loud one in
// development, where a learner (or a translator) will actually see it.

export type MessageCatalog = Record<string, Record<string, string>>;

export const MESSAGE_CATALOG = new InjectionToken<MessageCatalog>("MESSAGE_CATALOG");

export const FALLBACK_LOCALE = new InjectionToken<string>("FALLBACK_LOCALE", {
  factory: (): string => "en",
});

@Injectable()
export class TranslationService {
  private readonly catalog = inject(MESSAGE_CATALOG);
  private readonly fallbackLocale = inject(FALLBACK_LOCALE);

  /** Starts on the fallback locale — there is no "current user" locale until something sets one. */
  readonly locale = signal(this.fallbackLocale);

  /**
   * TODO: implement setLocale.
   *   - If `locale` has no entry at all in the catalog, throw a RangeError and leave `this.locale`
   *     unchanged (a rejected switch must not partially apply).
   *   - Otherwise, set `this.locale` to it.
   */
  setLocale(locale: string): void {
    throw new Error("TODO: implement setLocale");
  }

  /**
   * TODO: implement translate.
   *   - Look up `key` in the CURRENT locale's catalog. If it's missing there, fall back to
   *     FALLBACK_LOCALE's catalog for the same key.
   *   - If the key is missing in BOTH, throw a RangeError.
   *   - Replace every `{{name}}` placeholder in the resolved template with `String(params[name])`.
   *     If `name` is not a key of `params`, throw a RangeError — do not leave `{{name}}` in the
   *     output.
   */
  translate(key: string, params: Record<string, string | number> = {}): string {
    throw new Error("TODO: implement translate");
  }
}
