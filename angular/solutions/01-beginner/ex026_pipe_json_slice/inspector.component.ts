import { JsonPipe, KeyValuePipe, SlicePipe, UpperCasePipe } from "@angular/common";
import { Component, signal } from "@angular/core";

// Exercise 026 — JsonPipe, SlicePipe and KeyValuePipe (reference solution).

export interface Config {
  readonly name: string;
  readonly retries: number;
  readonly debug: boolean;
}

@Component({
  selector: "app-inspector",
  standalone: true,
  imports: [JsonPipe, SlicePipe, KeyValuePipe, UpperCasePipe],
  template: `
    <pre class="json">{{ config() | json }}</pre>
    <p class="first-two">{{ tags() | slice: 0 : 2 }}</p>
    <p class="last-two">{{ tags() | slice: -2 }}</p>
    <!-- Pipes chain left to right: slice the string, then upper-case the result. -->
    <p class="initials">{{ title() | slice: 0 : 3 | uppercase }}</p>
    <ul class="sorted">
      @for (entry of scores() | keyvalue; track entry.key) {
        <li class="entry">{{ entry.key }}={{ entry.value }}</li>
      }
    </ul>
    <ul class="ranked">
      @for (entry of scores() | keyvalue: byValueDescending; track entry.key) {
        <li class="ranked-entry">{{ entry.key }}={{ entry.value }}</li>
      }
    </ul>
  `,
})
export class InspectorComponent {
  readonly config = signal<Config>({ name: "api", retries: 3, debug: false });

  readonly tags = signal<readonly string[]>(["alpha", "beta", "gamma", "delta"]);

  readonly title = signal("angular");

  readonly scores = signal<Record<string, number>>({ zoe: 40, adam: 90, mia: 70 });

  // An arrow *field*, so the identity is stable for the life of the component. A method
  // would work too; `(a, b) => …` written inline in the template would not.
  byValueDescending = (
    a: { key: string; value: number },
    b: { key: string; value: number },
  ): number => b.value - a.value;

  summary(): string {
    return this.tags().join(", ");
  }
}
