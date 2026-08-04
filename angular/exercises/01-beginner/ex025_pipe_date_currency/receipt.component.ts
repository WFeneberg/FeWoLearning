import { Component, signal } from "@angular/core";

// Exercise 025 — DatePipe, CurrencyPipe and DecimalPipe (beginner).
// Goal:   format dates and money in the template instead of in the class.
// Drills: | date with a format string, pinning the timezone, | currency with a code and
//         display option, | number with digit info, and LOCALE_ID changing all of them.
// Passes: when `npx jest exercises/01-beginner/ex025_pipe_date_currency` is green.
//
// These pipes come from CommonModule, so a standalone component imports the ones it uses
// (DatePipe, CurrencyPipe, DecimalPipe) — or CommonModule wholesale, which pulls in
// everything.
//
// The trap this exercise exists for: `| date` formats in the *runtime's local timezone*
// by default, so the same code renders a different day depending on where it runs, and a
// test that passes on your machine fails in CI. Pass the timezone explicitly — the third
// argument — whenever the exact instant matters. The digits argument to `| number` is
// "minIntegerDigits.minFractionDigits-maxFractionDigits", which is easy to misread.
//
// Locale is a DI value, not a pipe argument: LOCALE_ID decides the decimal separator, the
// month names and where the currency symbol goes. The spec provides "de-DE" in one test to
// show the same template producing German output.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="iso">{{ placedAt() | date: "yyyy-MM-dd" : "UTC" }}</p>
//   <p class="long">{{ placedAt() | date: "d MMMM y, HH:mm" : "UTC" }}</p>
//   <p class="local">{{ placedAt() | date: "yyyy-MM-dd HH:mm" : "+0200" }}</p>
//   <p class="total">{{ total() | currency: currencyCode() }}</p>
//   <p class="code">{{ total() | currency: currencyCode() : "code" }}</p>
//   <p class="plain">{{ total() | number: "1.2-2" }}</p>
//   <p class="rounded">{{ total() | number: "1.0-0" }}</p>
@Component({
  selector: "app-receipt",
  standalone: true,
  // TODO: import the pipes this template uses.
  template: `<p>TODO: render the receipt — see the template contract above</p>`,
})
export class ReceiptComponent {
  /** 2026-03-14T22:30:00Z — deliberately late in the UTC day, so a timezone slip shows. */
  readonly placedAt = signal(new Date("2026-03-14T22:30:00Z"));

  readonly total = signal(1234.567);

  readonly currencyCode = signal("EUR");

  /**
   * A plain-TypeScript description, for contrast with the pipes above.
   *
   * `"1234.57 EUR"` — the total fixed to two decimals, a space, then the code. Formatting
   * in the class like this is the thing the pipes save you from doing by hand.
   */
  describe(): string {
    throw new Error("TODO: implement describe");
  }
}
