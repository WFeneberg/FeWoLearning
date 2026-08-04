import { CurrencyPipe, DatePipe, DecimalPipe } from "@angular/common";
import { Component, signal } from "@angular/core";

// Exercise 025 — DatePipe, CurrencyPipe and DecimalPipe (reference solution).
@Component({
  selector: "app-receipt",
  standalone: true,
  // Only the three pipes actually used, rather than all of CommonModule.
  imports: [DatePipe, CurrencyPipe, DecimalPipe],
  template: `
    <!-- The third argument pins the timezone. Without it these render differently
         depending on where the code happens to be running. -->
    <p class="iso">{{ placedAt() | date: "yyyy-MM-dd" : "UTC" }}</p>
    <p class="long">{{ placedAt() | date: "d MMMM y, HH:mm" : "UTC" }}</p>
    <p class="local">{{ placedAt() | date: "yyyy-MM-dd HH:mm" : "+0200" }}</p>
    <p class="total">{{ total() | currency: currencyCode() }}</p>
    <p class="code">{{ total() | currency: currencyCode() : "code" }}</p>
    <!-- minIntegerDigits.minFractionDigits-maxFractionDigits -->
    <p class="plain">{{ total() | number: "1.2-2" }}</p>
    <p class="rounded">{{ total() | number: "1.0-0" }}</p>
  `,
})
export class ReceiptComponent {
  readonly placedAt = signal(new Date("2026-03-14T22:30:00Z"));

  readonly total = signal(1234.567);

  readonly currencyCode = signal("EUR");

  describe(): string {
    // toFixed() is locale-blind: it always uses a dot, wherever the app is running.
    return `${this.total().toFixed(2)} ${this.currencyCode()}`;
  }
}
