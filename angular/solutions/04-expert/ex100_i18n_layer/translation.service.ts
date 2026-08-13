import { Injectable, InjectionToken, inject, signal } from "@angular/core";

// Exercise 100 — an i18n layer: message catalog, interpolation, runtime locale switching
// (reference solution).

export type MessageCatalog = Record<string, Record<string, string>>;

export const MESSAGE_CATALOG = new InjectionToken<MessageCatalog>("MESSAGE_CATALOG");

export const FALLBACK_LOCALE = new InjectionToken<string>("FALLBACK_LOCALE", {
  factory: (): string => "en",
});

const PLACEHOLDER = /\{\{\s*(\w+)\s*\}\}/g;

@Injectable()
export class TranslationService {
  private readonly catalog = inject(MESSAGE_CATALOG);
  private readonly fallbackLocale = inject(FALLBACK_LOCALE);

  readonly locale = signal(this.fallbackLocale);

  setLocale(locale: string): void {
    if (!(locale in this.catalog)) {
      throw new RangeError(`unknown locale: ${locale}`);
    }
    this.locale.set(locale);
  }

  translate(key: string, params: Record<string, string | number> = {}): string {
    const template = this.catalog[this.locale()]?.[key] ?? this.catalog[this.fallbackLocale]?.[key];
    if (template === undefined) {
      throw new RangeError(
        `missing translation for key "${key}" in locale "${this.locale()}" ` +
          `(and fallback "${this.fallbackLocale}")`,
      );
    }

    return template.replace(PLACEHOLDER, (_match, name: string) => {
      if (!(name in params)) {
        throw new RangeError(`translate("${key}"): missing interpolation param "${name}"`);
      }
      return String(params[name]);
    });
  }
}
