import { Component, computed, input, signal } from "@angular/core";

// Exercise 075 — OnPush change detection and the immutability it demands (advanced).
// Goal:   make a component skip re-rendering unless something it actually depends on changed —
//         and understand exactly what "changed" means once OnPush is in charge.
// Drills: ChangeDetectionStrategy.OnPush, input() signals, and why mutating a collection in place
//         is invisible to it while replacing the collection is not.
// Passes: when `npx jest exercises/03-advanced/ex075_onpush_change_detection` is green.
//
// The Default change-detection strategy checks every binding in a component's template on every
// tick, unconditionally — cheap for a small app, expensive once the tree gets large. OnPush trades
// that blanket guarantee for a narrower one: a component is (re)checked only when Angular can prove
// something relevant happened — one of its `input()` signals received a genuinely new reference, an
// event originated from inside its own template, or a signal it reads was written to. Angular
// proves the first case with a plain `Object.is` comparison against the previous value.
//
// That comparison is exactly why immutability stops being a style preference and becomes a
// correctness requirement. Push a new item onto an array that a parent already handed to an OnPush
// child, and the child's `items` input still holds the very same array reference it had before —
// `Object.is(oldArray, newArray)` is true, so as far as OnPush is concerned nothing happened, and the
// view is left stale. Give the child a fresh array instead (`[...items, newItem]`) and the
// reference visibly differs, so OnPush lets the update through. Nothing here is unique to
// arrays — the same rule applies to any object passed as an input.
//
// Note what OnPush does *not* block: a signal written to from inside the component itself (a click
// handler calling `.set()` or `.update()`) always marks that component for the next check, strategy
// or no strategy — OnPush only restricts what counts as an external reason to look at a component,
// never a component's own internal reactivity.

export interface Todo {
  id: number;
  text: string;
  done: boolean;
}

// TODO: give this component `changeDetection: ChangeDetectionStrategy.OnPush` — without it, every
// test below that expects a mutated array to leave the view untouched will fail, because Default
// strategy re-checks this template on every detectChanges() call regardless of what changed.
@Component({
  selector: "app-todo-summary",
  standalone: true,
  template: `
    <p class="count">{{ items().length }} items, {{ doneCount() }} done</p>
    <button class="toggle-collapsed" type="button" (click)="toggleCollapsed()">Toggle</button>
    <p class="collapsed-state">{{ collapsed() ? "collapsed" : "expanded" }}</p>
  `,
})
export class TodoSummaryComponent {
  readonly items = input<Todo[]>([]);
  readonly collapsed = signal(false);

  /** TODO: how many of `items()` have `done === true`. */
  readonly doneCount = computed<number>(() => {
    throw new Error("TODO: implement doneCount");
  });

  /** TODO: flip `collapsed` — an internal signal write, which always triggers a re-render. */
  toggleCollapsed(): void {
    throw new Error("TODO: implement toggleCollapsed");
  }
}
