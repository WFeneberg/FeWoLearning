import { NgTemplateOutlet } from "@angular/common";
import { Component, signal } from "@angular/core";

// Exercise 028 — ng-template and NgTemplateOutlet (reference solution).

export interface Row {
  readonly name: string;
  readonly note: string;
}

@Component({
  selector: "app-row-list",
  standalone: true,
  imports: [NgTemplateOutlet],
  template: `
    <!-- Definitions. Neither of these renders anything by being here. -->
    <ng-template #compact let-item let-index="index">
      <span class="compact">{{ index }}:{{ item.name }}</span>
    </ng-template>

    <ng-template #detailed let-item let-index="index" let-total="total">
      <span class="detailed">{{ index }}/{{ total }} {{ item.name }} — {{ item.note }}</span>
    </ng-template>

    <div class="rows">
      @for (item of items(); track item.name; let i = $index) {
        <ng-container
          [ngTemplateOutlet]="dense() ? compact : detailed"
          [ngTemplateOutletContext]="contextFor(item, i)"
        />
      }
    </div>

    <div class="preview">
      <ng-container [ngTemplateOutlet]="compact" [ngTemplateOutletContext]="previewContext()" />
    </div>
  `,
})
export class RowListComponent {
  readonly items = signal<readonly Row[]>([
    { name: "alpha", note: "first" },
    { name: "beta", note: "second" },
  ]);

  readonly dense = signal(true);

  contextFor(item: Row, index: number): Record<string, unknown> {
    // `$implicit` is what a bare `let-item` picks up; the rest are read by name.
    return { $implicit: item, index, total: this.items().length };
  }

  previewContext(): Record<string, unknown> {
    return { $implicit: { name: "sample", note: "n/a" }, index: 0 };
  }
}
