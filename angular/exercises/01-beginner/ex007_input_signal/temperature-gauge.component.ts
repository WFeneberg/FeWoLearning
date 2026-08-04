import { Component } from "@angular/core";

// Exercise 007 — TemperatureGaugeComponent (beginner).
// Goal:   the same inputs as exercise 006, but as *signals* instead of decorators.
// Drills: input(), input.required(), reading an input by calling it, the alias and
//         transform options, and deriving a computed straight from an input.
// Passes: when `npx jest exercises/01-beginner/ex007_input_signal` is green.
//
// What signal inputs buy you over @Input(): the value is a signal, so a computed can
// depend on it directly and no ngOnChanges is needed to react to a change. They are
// *read-only* signals — the parent owns the value, so there is no set() or update().
//
// input.required<T>() takes no default. Reading it before the parent has bound anything
// throws NG0950 at runtime, which is a far louder failure than the silent `undefined` a
// decorator-based required input leaves behind.
//
// Each field below is declared as a plain callable so the stub compiles. Replace the
// declaration — not just the body — with the real input() call.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <h3 class="label">{{ label() }}</h3>
//   <p class="reading">{{ reading() }}</p>
//   <p class="mode">{{ compact() ? "compact" : "full" }}</p>
@Component({
  selector: "app-temperature-gauge",
  standalone: true,
  template: `<p>TODO: render the gauge — see the template contract above</p>`,
})
export class TemperatureGaugeComponent {
  /** TODO: a required string input — no default. */
  readonly label: () => string = () => {
    throw new Error("TODO: declare label as a required signal input");
  };

  /** TODO: an optional input defaulting to 0 that coerces its value to a number. */
  readonly celsius: () => number = () => {
    throw new Error("TODO: declare celsius as a signal input");
  };

  /** TODO: an optional input defaulting to "C". */
  readonly unit: () => "C" | "F" = () => {
    throw new Error("TODO: declare unit as a signal input");
  };

  /** TODO: an input named `digits` in templates, defaulting to 1, coerced to a number. */
  readonly precision: () => number = () => {
    throw new Error("TODO: declare precision as an aliased signal input");
  };

  /** TODO: an input defaulting to false that coerces an attribute-style value. */
  readonly compact: () => boolean = () => {
    throw new Error("TODO: declare compact as a signal input");
  };

  /**
   * TODO: a computed reading, derived from the inputs above.
   *
   * The celsius value converted to `unit` (F is `c * 9 / 5 + 32`), fixed to `precision`
   * decimals, then the degree sign and the unit: 21.456 at precision 2 in C is
   * "21.46 °C". In compact mode drop the space before the degree sign: "21.46°C".
   */
  readonly reading: () => string = () => {
    throw new Error("TODO: implement the reading computed");
  };
}
