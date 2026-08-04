import { Component, signal } from "@angular/core";

// Exercise 011 — TaskListComponent (reference solution).

export interface Task {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Component({
  selector: "app-task-list",
  standalone: true,
  template: `
    <p class="summary">{{ doneCount() }} of {{ tasks().length }} done</p>
    <ul class="tasks">
      <!-- track by id, not $index: identity is what lets Angular move a node instead
           of rebuilding it when the list is reordered. -->
      @for (task of tasks(); track task.id) {
        <li
          class="task"
          [class.first]="$first"
          [class.last]="$last"
          [class.even]="$even"
          [attr.data-id]="task.id"
        >{{ $index }}: {{ task.title }} ({{ $count }})</li>
      } @empty {
        <li class="empty">No tasks</li>
      }
    </ul>
  `,
})
export class TaskListComponent {
  readonly tasks = signal<readonly Task[]>([]);

  doneCount(): number {
    return this.tasks().filter((task) => task.done).length;
  }

  prepend(task: Task): void {
    // A new array, not unshift(): a signal compares by reference.
    this.tasks.update((tasks) => [task, ...tasks]);
  }
}
