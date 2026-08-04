import { Component, signal } from "@angular/core";

// Exercise 005 — ClickTrackerComponent (beginner).
// Goal:   wire DOM events to handler methods and take control of the event object.
// Drills: (click)="handler()", passing the DOM event with $event, reading modifier
//         keys off it, preventDefault() to stop the browser's own behaviour, and
//         stopPropagation() to keep an event off an ancestor's handler.
// Passes: when `npx jest exercises/01-beginner/ex005_event_binding_click` is green.
//
// $event is the real DOM event, not an Angular wrapper — which is why the handlers can
// call preventDefault() and stopPropagation() on it directly. Take the parameter only
// when you need it: `(click)="reset()"` is better than threading an event you ignore.
//
// Template contract the spec asserts (classes are the query hooks — keep the nesting):
//   <p class="taps">Taps: {{ taps() }}</p>
//   <p class="outer-taps">Outer: {{ outerTaps() }}</p>
//   <p class="modifiers">{{ modifiers().join(",") }}</p>
//   <button class="tap" type="button" (click)="tap($event)">Tap</button>
//   <a class="link" href="/nope" (click)="follow($event)">Details</a>
//   <div class="outer" (click)="outerTap()">
//     <button class="inner" type="button" (click)="innerTap($event)">Inner</button>
//   </div>
//   <button class="reset" type="button" (click)="reset()">Reset</button>
@Component({
  selector: "app-click-tracker",
  standalone: true,
  template: `<p>TODO: render the tracker — see the template contract above</p>`,
})
export class ClickTrackerComponent {
  readonly taps = signal(0);
  readonly outerTaps = signal(0);
  readonly modifiers = signal<readonly string[]>([]);

  /**
   * Count the tap and append one label describing how it was clicked:
   * "shift" when shiftKey is held, otherwise "ctrl" when ctrlKey is held,
   * otherwise "plain".
   */
  tap(event: MouseEvent): void {
    throw new Error("TODO: implement tap");
  }

  /**
   * Stop the link from navigating and append "blocked" to the modifiers.
   * The tap count must not change — this is not a tap.
   */
  follow(event: Event): void {
    throw new Error("TODO: implement follow");
  }

  /** Count the tap, and keep the event from reaching the surrounding div. */
  innerTap(event: MouseEvent): void {
    throw new Error("TODO: implement innerTap");
  }

  /** Count a click that landed on the surrounding div. */
  outerTap(): void {
    throw new Error("TODO: implement outerTap");
  }

  /** Back to zero taps, zero outer taps and no modifiers. */
  reset(): void {
    throw new Error("TODO: implement reset");
  }
}
