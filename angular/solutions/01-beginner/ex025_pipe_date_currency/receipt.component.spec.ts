import { LOCALE_ID } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { registerLocaleData } from "@angular/common";
import localeDe from "@angular/common/locales/de";
import { ReceiptComponent } from "./receipt.component";

registerLocaleData(localeDe);

describe("ReceiptComponent", () => {
  let fixture: ComponentFixture<ReceiptComponent>;
  let component: ReceiptComponent;

  const query = <T extends HTMLElement>(selector: string): T => {
    const found = fixture.nativeElement.querySelector(selector) as T | null;
    if (found === null) {
      throw new Error(`no element matched "${selector}" — is the template contract implemented?`);
    }
    return found;
  };

  /**
   * Read an element's text, normalising U+00A0 to a plain space.
   *
   * Angular's locale data separates an amount from its currency symbol with a
   * *non-breaking* space in many locales (de-DE among them), so a naive comparison
   * against a string typed with a normal space fails while looking identical.
   */
  const text = (selector: string): string =>
    (query(selector).textContent ?? "").replace(/\u00A0/g, " ").trim();

  const build = async (providers: unknown[] = []): Promise<void> => {
    TestBed.resetTestingModule();
    await TestBed.configureTestingModule({
      imports: [ReceiptComponent],
      providers: providers as never,
    }).compileComponents();
    fixture = TestBed.createComponent(ReceiptComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  };

  beforeEach(async () => {
    await build();
  });

  it("formats a date with an explicit timezone", () => {
    expect(text("p.iso")).toBe("2026-03-14");
  });

  it("formats month names and time", () => {
    expect(text("p.long")).toBe("14 March 2026, 22:30");
  });

  it("shifts the date when the timezone shifts", () => {
    // 22:30 UTC is already the next day at +0200 — which is exactly why the timezone
    // argument is not optional in anything that has to be correct.
    expect(text("p.local")).toBe("2026-03-15 00:30");
  });

  it("re-formats when the date changes", () => {
    component.placedAt.set(new Date("2026-12-01T09:05:00Z"));
    fixture.detectChanges();

    expect(text("p.iso")).toBe("2026-12-01");
    expect(text("p.long")).toBe("1 December 2026, 09:05");
  });

  it("formats currency with its symbol", () => {
    expect(text("p.total")).toBe("€1,234.57");
  });

  it("formats currency with its code instead", () => {
    expect(text("p.code")).toBe("EUR1,234.57");
  });

  it("follows the currency code", () => {
    component.currencyCode.set("USD");
    fixture.detectChanges();

    expect(text("p.total")).toBe("$1,234.57");
  });

  it("formats a number to a fixed number of decimals", () => {
    expect(text("p.plain")).toBe("1,234.57");
  });

  it("rounds when no decimals are allowed", () => {
    expect(text("p.rounded")).toBe("1,235");
  });

  it("pads to the minimum fraction digits", () => {
    component.total.set(5);
    fixture.detectChanges();

    // "1.2-2" means at least one integer digit and exactly two fraction digits.
    expect(text("p.plain")).toBe("5.00");
  });

  it("describes the total in plain TypeScript", () => {
    expect(component.describe()).toBe("1234.57 EUR");
  });

  it("follows the code in the plain description too", () => {
    component.currencyCode.set("USD");

    expect(component.describe()).toBe("1234.57 USD");
  });

  it("changes every format when the locale changes", async () => {
    await build([{ provide: LOCALE_ID, useValue: "de-DE" }]);

    // Same template, German conventions: swapped separators, German month name, and the
    // symbol after the amount.
    expect(text("p.plain")).toBe("1.234,57");
    expect(text("p.long")).toBe("14 März 2026, 22:30");
    expect(text("p.total")).toBe("1.234,57 €");
  });

  it("leaves the hand-rolled description untouched by the locale", async () => {
    await build([{ provide: LOCALE_ID, useValue: "de-DE" }]);

    // toFixed() knows nothing about locale — another reason to let the pipes do it.
    expect(component.describe()).toBe("1234.57 EUR");
  });
});
