import { ChangeDetectionStrategy, Component, computed, input, signal } from "@angular/core";

// Exercise 075 — OnPush change detection and the immutability it demands (reference solution).

export interface Todo {
  id: number;
  text: string;
  done: boolean;
}

@Component({
  selector: "app-todo-summary",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <p class="count">{{ items().length }} items, {{ doneCount() }} done</p>
    <button class="toggle-collapsed" type="button" (click)="toggleCollapsed()">Toggle</button>
    <p class="collapsed-state">{{ collapsed() ? "collapsed" : "expanded" }}</p>
  `,
})
export class TodoSummaryComponent {
  readonly items = input<Todo[]>([]);
  readonly collapsed = signal(false);

  readonly doneCount = computed<number>(() => this.items().filter((t) => t.done).length);

  toggleCollapsed(): void {
    this.collapsed.update((v) => !v);
  }
}
