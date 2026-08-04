import { Component, signal } from "@angular/core";

// Exercise 028 — ng-template and NgTemplateOutlet (beginner).
// Goal:   declare a chunk of markup once and stamp it out where and when you choose.
// Drills: <ng-template #name>, [ngTemplateOutlet], [ngTemplateOutletContext], the
//         $implicit context key and its `let-x` shorthand, and named context keys.
// Passes: when `npx jest exercises/01-beginner/ex028_ng_template_outlet` is green.
//
// An <ng-template> renders nothing on its own — it is a *definition*. Angular keeps it as
// a TemplateRef, and NgTemplateOutlet is what instantiates it, as many times as you like
// and with different data each time. That is the machinery underneath @if and @for, and
// the reason a component can let its caller supply the markup for a row.
//
// Context is a plain object whose keys become the template's variables. `let-item` with no
// value is shorthand for the key named `$implicit` — every template gets at most one of
// those — and `let-index="index"` reads the key literally called "index". A key that is
// absent is simply undefined, which is why a typo here fails silently rather than loudly.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <ng-template #compact let-item let-index="index">
//     <span class="compact">{{ index }}:{{ item.name }}</span>
//   </ng-template>
//
//   <ng-template #detailed let-item let-index="index" let-total="total">
//     <span class="detailed">{{ index }}/{{ total }} {{ item.name }} — {{ item.note }}</span>
//   </ng-template>
//
//   <div class="rows">
//     @for (item of items(); track item.name; let i = $index) {
//       <ng-container
//         [ngTemplateOutlet]="dense() ? compact : detailed"
//         [ngTemplateOutletContext]="contextFor(item, i)"
//       />
//     }
//   </div>
//
//   <!-- The same definition again, stamped once with fixed data. -->
//   <div class="preview">
//     <ng-container [ngTemplateOutlet]="compact" [ngTemplateOutletContext]="previewContext()" />
//   </div>

export interface Row {
  readonly name: string;
  readonly note: string;
}

@Component({
  selector: "app-row-list",
  standalone: true,
  // TODO: import NgTemplateOutlet.
  template: `<p>TODO: render the list — see the template contract above</p>`,
})
export class RowListComponent {
  readonly items = signal<readonly Row[]>([
    { name: "alpha", note: "first" },
    { name: "beta", note: "second" },
  ]);

  readonly dense = signal(true);

  /**
   * The context object for one row: the item as `$implicit`, plus `index` and `total`.
   *
   * `total` is the number of items, so the detailed template can render "1/2".
   */
  contextFor(item: Row, index: number): Record<string, unknown> {
    throw new Error("TODO: implement contextFor");
  }

  /** A fixed context for the preview: a row named "sample" with note "n/a", at index 0. */
  previewContext(): Record<string, unknown> {
    throw new Error("TODO: implement previewContext");
  }
}
