import { Component, signal } from "@angular/core";

// Exercise 026 — JsonPipe, SlicePipe and KeyValuePipe (beginner).
// Goal:   inspect and trim data in the template with the remaining built-in pipes.
// Drills: | json for debugging, | slice for arrays and strings, | keyvalue over an object
//         (including its default sorting and a custom comparator), and | uppercase.
// Passes: when `npx jest exercises/01-beginner/ex026_pipe_json_slice` is green.
//
// `| json` is a debugging tool, not a display tool: it is JSON.stringify with an indent,
// so it is perfect in a scratch template and wrong in a user-facing one.
//
// `| slice` takes the same arguments as Array.prototype.slice, negatives included, and
// works on strings too. It returns a *new* array every time it runs, which is why an
// `@for` over a sliced list needs a `track` that identifies the item rather than the array
// position (exercise 011).
//
// `| keyvalue` turns an object into `{key, value}` entries and — the part that surprises
// people — **sorts by key by default**. Insertion order is not preserved unless you pass a
// comparator, and passing `null` keeps the original order.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <pre class="json">{{ config() | json }}</pre>
//   <p class="first-two">{{ tags() | slice: 0 : 2 }}</p>
//   <p class="last-two">{{ tags() | slice: -2 }}</p>
//   <p class="initials">{{ title() | slice: 0 : 3 | uppercase }}</p>
//   <ul class="sorted">
//     @for (entry of scores() | keyvalue; track entry.key) {
//       <li class="entry">{{ entry.key }}={{ entry.value }}</li>
//     }
//   </ul>
//   <ul class="ranked">
//     @for (entry of scores() | keyvalue: byValueDescending; track entry.key) {
//       <li class="ranked-entry">{{ entry.key }}={{ entry.value }}</li>
//     }
//   </ul>

export interface Config {
  readonly name: string;
  readonly retries: number;
  readonly debug: boolean;
}

@Component({
  selector: "app-inspector",
  standalone: true,
  // TODO: import the pipes this template uses.
  template: `<p>TODO: render the inspector — see the template contract above</p>`,
})
export class InspectorComponent {
  readonly config = signal<Config>({ name: "api", retries: 3, debug: false });

  readonly tags = signal<readonly string[]>(["alpha", "beta", "gamma", "delta"]);

  readonly title = signal("angular");

  /** Deliberately not in alphabetical order, so keyvalue's default sorting is visible. */
  readonly scores = signal<Record<string, number>>({ zoe: 40, adam: 90, mia: 70 });

  /**
   * TODO: a comparator for | keyvalue that ranks by value, highest first.
   *
   * KeyValuePipe hands it two `{key, value}` entries and wants the usual negative / zero /
   * positive answer. It must be a stable class member (a method or an arrow field), not a
   * fresh function per render, or the pipe re-sorts on every change-detection pass.
   */
  byValueDescending = (
    a: { key: string; value: number },
    b: { key: string; value: number },
  ): number => {
    throw new Error("TODO: implement byValueDescending");
  };

  /** The tags joined with ", " — plain TypeScript, for contrast with | slice. */
  summary(): string {
    throw new Error("TODO: implement summary");
  }
}
