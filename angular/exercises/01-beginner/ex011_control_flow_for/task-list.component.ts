import { Component, signal } from "@angular/core";

// Exercise 011 — TaskListComponent (beginner).
// Goal:   render a list with @for, and learn what `track` is actually for.
// Drills: @for with a mandatory `track`, the implicit $index / $count / $first / $last /
//         $even variables, the @empty block, and DOM reuse across reorderings.
// Passes: when `npx jest exercises/01-beginner/ex011_control_flow_for` is green.
//
// `track` is not optional and not decoration. It tells Angular how to match an item in
// the new array to a DOM node it already built. Track by a stable identity (`task.id`)
// and reordering the array *moves* the existing nodes; track by `$index` and every node
// at a shifted position is re-rendered instead. The spec proves this by holding on to a
// DOM node and checking the same object survives a prepend — which is also why losing
// input focus or restarting a CSS animation on every keystroke is a tracking bug.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="summary">{{ doneCount() }} of {{ tasks().length }} done</p>
//   <ul class="tasks">
//     @for (task of tasks(); track task.id) {
//       <li
//         class="task"
//         [class.first]="$first"
//         [class.last]="$last"
//         [class.even]="$even"
//         [attr.data-id]="task.id"
//       >{{ $index }}: {{ task.title }} ({{ $count }})</li>
//     } @empty {
//       <li class="empty">No tasks</li>
//     }
//   </ul>

export interface Task {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Component({
  selector: "app-task-list",
  standalone: true,
  template: `<p>TODO: render the list — see the template contract above</p>`,
})
export class TaskListComponent {
  readonly tasks = signal<readonly Task[]>([]);

  /** How many tasks are done. */
  doneCount(): number {
    throw new Error("TODO: implement doneCount");
  }

  /** Add a task at the *front* of the list, without mutating the existing array. */
  prepend(task: Task): void {
    throw new Error("TODO: implement prepend");
  }
}
