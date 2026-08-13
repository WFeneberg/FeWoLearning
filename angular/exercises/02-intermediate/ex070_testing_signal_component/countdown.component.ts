import { Component, computed, signal, input } from "@angular/core";

// Exercise 070 — asserting signal state through a fixture (intermediate).
// Goal:   know which of three things you are actually asserting when you test a signal component:
//         the signal itself, the rendered DOM, or a signal *input*.
// Drills: reading a signal straight off `fixture.componentInstance` (always current, no render
//         needed), `fixture.detectChanges()` as the missing step before the DOM agrees with it, and
//         `fixture.componentRef.setInput()` as the only way to change a signal input from a test.
// Passes: when `npx jest exercises/02-intermediate/ex070_testing_signal_component` is green.
//
// A signal read on the component instance is correct the instant you read it — computing it does
// not involve the DOM at all. That makes `component.remaining()` the fast, render-free way to assert
// state. The DOM is a second, separate step: Angular does not re-render on every signal write inside
// a test, so a test that changes state and then checks `nativeElement` without calling
// `fixture.detectChanges()` first is checking yesterday's render.
//
// A signal input (`input()`) is deliberately not a plain settable property — `component.startFrom =
// 5` is a type error, on purpose, because a real caller can only ever bind it, never poke it after
// the fact. In a test there is no host template to bind through, so the fixture stands in for the
// caller: `fixture.componentRef.setInput("startFrom", 5)` is the supported way to change it, and it
// re-runs whatever the input change should trigger (here, a linkedSignal resetting the countdown)
// the same way a real re-binding would.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="remaining">{{ display() }}</p>
//   <button class="tick" type="button" (click)="tick()">Tick</button>

@Component({
  selector: "app-countdown",
  standalone: true,
  template: `<p>TODO: render the countdown — see the template contract above</p>`,
})
export class CountdownComponent {
  /** Where the countdown starts. Bound by the caller, changed in tests via setInput(). */
  readonly startFrom = input.required<number>();

  /**
   * TODO: a linkedSignal (exercise 066) seeded from startFrom.
   *
   * linkedSignal is the point of this exercise's component: whenever startFrom changes, remaining
   * resets to the new value, exactly the way a fresh binding should restart the countdown.
   */
  readonly remaining = signal(0);

  /** TODO: whether the countdown has reached zero. */
  readonly finished = computed(() => false);

  /** TODO: "Done" once finished, otherwise the remaining count as a string. */
  readonly display = computed(() => "");

  /** One tick down, never below zero. */
  tick(): void {
    throw new Error("TODO: implement tick");
  }
}
