import { Component, signal } from "@angular/core";

// Exercise 016 — VolumeDialComponent (beginner).
// Goal:   own a value the parent can also write to, with model().
// Drills: model(), model.required(), reading and writing it like a WritableSignal, the
//         implicit `xChange` output that makes [(x)] work, and clamping a parent's write.
// Passes: when `npx jest exercises/01-beginner/ex016_model_signal` is green.
//
// model() is input() and output() fused into one writable signal. Declaring `value` gives
// you an input named `value` *and* an output named `valueChange`, which is exactly the
// convention [(value)] looks for — so a parent binds [(value)]="…" and the child just
// calls value.set(). No EventEmitter, no manual re-emit, and unlike ngModel (exercise
// 015) the child side is a signal rather than a plain property.
//
// The catch worth knowing: writing through model() emits the change output, so a parent
// bound with [(value)] is updated. Writing the *same* value is still a write and still
// emits — clamping in the child means deciding whether to write at all.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="level">{{ label() }}: {{ level() }}</p>
//   <p class="muted">{{ muted() ? "muted" : "live" }}</p>
//   <button class="up" type="button" (click)="up()">+</button>
//   <button class="down" type="button" (click)="down()">-</button>
//   <button class="mute" type="button" (click)="toggleMute()">Mute</button>

@Component({
  selector: "app-volume-dial",
  standalone: true,
  template: `<p>TODO: render the dial — see the template contract above</p>`,
})
export class VolumeDialComponent {
  // Both fields below are plain local signals so the stub compiles and the methods have
  // something to read. A signal is writable and callable just like a model — what it is
  // not is *connected to a parent*, so nothing outside can bind or observe it. Replace
  // each declaration, not just the method bodies.

  /** TODO: a required model — an input `label` plus an output `labelChange`. */
  readonly label = signal("");

  /** TODO: a two-way model defaulting to 50, writable from inside *and* out. */
  readonly level = signal(50);

  /** A plain local signal, for contrast: nothing outside this component can write it. */
  readonly muted = signal(false);

  /** Raise the level by 10, stopping at 100. At the ceiling, do not write at all. */
  up(): void {
    throw new Error("TODO: implement up");
  }

  /** Lower the level by 10, stopping at 0. At the floor, do not write at all. */
  down(): void {
    throw new Error("TODO: implement down");
  }

  /** Flip muted. Muting does not change the level — it is remembered for unmuting. */
  toggleMute(): void {
    throw new Error("TODO: implement toggleMute");
  }

  /** The level the speaker actually plays at: 0 while muted, otherwise the level. */
  effective(): number {
    throw new Error("TODO: implement effective");
  }
}
