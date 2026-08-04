import { booleanAttribute, Component, computed, input, numberAttribute } from "@angular/core";

// Exercise 007 — TemperatureGaugeComponent (reference solution).
@Component({
  selector: "app-temperature-gauge",
  standalone: true,
  template: `
    <h3 class="label">{{ label() }}</h3>
    <p class="reading">{{ reading() }}</p>
    <p class="mode">{{ compact() ? "compact" : "full" }}</p>
  `,
})
export class TemperatureGaugeComponent {
  // No default, no initialiser: reading this before the parent binds it throws NG0950.
  readonly label = input.required<string>();

  readonly celsius = input(0, { transform: numberAttribute });

  readonly unit = input<"C" | "F">("C");

  // `alias` renames only the public, template-facing name.
  readonly precision = input(1, { alias: "digits", transform: numberAttribute });

  readonly compact = input(false, { transform: booleanAttribute });

  // The payoff of signal inputs: a computed depends on them directly, so there is no
  // ngOnChanges and no chance of a stale derived value.
  readonly reading = computed<string>(() => {
    const celsius = this.celsius();
    const value = this.unit() === "F" ? celsius * 9 / 5 + 32 : celsius;
    const separator = this.compact() ? "" : " ";
    return `${value.toFixed(this.precision())}${separator}°${this.unit()}`;
  });
}
