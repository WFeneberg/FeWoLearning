import { TestBed } from "@angular/core/testing";
import {
  FALLBACK_LOCALE,
  MESSAGE_CATALOG,
  MessageCatalog,
  TranslationService,
} from "./translation.service";

const CATALOG: MessageCatalog = {
  en: {
    greeting: "Hello, {{name}}!",
    farewell: "Goodbye!",
  },
  de: {
    greeting: "Hallo, {{name}}!",
    onlyInGerman: "Nur auf Deutsch",
  },
  fr: {}, // registered locale, but no keys of its own yet
};

describe("TranslationService (message catalog, interpolation, runtime locale switching)", () => {
  let service: TranslationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TranslationService,
        { provide: MESSAGE_CATALOG, useValue: CATALOG },
        { provide: FALLBACK_LOCALE, useValue: "en" },
      ],
    });
    service = TestBed.inject(TranslationService);
  });

  it("defaults to the fallback locale, and interpolates params into its template", () => {
    expect(service.locale()).toBe("en");
    expect(service.translate("greeting", { name: "Ada" })).toBe("Hello, Ada!");
  });

  it("switches locale at runtime and reflects it in translate() output", () => {
    service.setLocale("de");

    expect(service.locale()).toBe("de");
    expect(service.translate("greeting", { name: "Ada" })).toBe("Hallo, Ada!");
  });

  it("falls back to the fallback locale when the current locale is missing that key", () => {
    service.setLocale("de");

    expect(service.translate("farewell")).toBe("Goodbye!"); // de has no "farewell" — falls back to en
  });

  it("falls back even when the current locale has no catalog entries at all", () => {
    service.setLocale("fr");

    expect(service.translate("farewell")).toBe("Goodbye!");
  });

  it("throws a RangeError when a key is missing in both the current locale and the fallback", () => {
    service.setLocale("fr");

    expect(() => service.translate("onlyInGerman")).toThrow(RangeError);
  });

  it("throws a RangeError when switching to a locale nothing registered a catalog for", () => {
    expect(() => service.setLocale("xx")).toThrow(RangeError);
  });

  it("leaves the current locale unchanged after a rejected switch", () => {
    service.setLocale("de");

    expect(() => service.setLocale("xx")).toThrow(RangeError);
    expect(service.locale()).toBe("de"); // the bad switch must not have partially applied
  });

  it("throws a RangeError when a required interpolation param is missing", () => {
    expect(() => service.translate("greeting", {})).toThrow(RangeError);
  });

  it("throws a RangeError for a key missing everywhere, even in the default locale", () => {
    expect(() => service.translate("neverRegisteredAnywhere")).toThrow(RangeError);
  });
});
