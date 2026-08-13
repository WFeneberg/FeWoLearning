import { Component, input } from "@angular/core";

// Exercise 090 — @for track correctness and DOM reuse (reference solution).

export interface TaggedItem {
  readonly id: number;
  readonly label: string;
}

@Component({
  selector: "app-tagged-list",
  standalone: true,
  template: `
    <ul>
      @for (item of items(); track trackItem(item)) {
        <li [attr.data-id]="item.id">
          {{ item.label }}
          <input class="local-input" />
        </li>
      }
    </ul>
  `,
})
export class TaggedListComponent {
  readonly items = input.required<readonly TaggedItem[]>();

  trackItem(item: TaggedItem): number {
    return item.id;
  }
}
