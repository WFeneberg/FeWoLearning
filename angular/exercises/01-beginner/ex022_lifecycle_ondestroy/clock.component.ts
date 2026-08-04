import { Component, inject, Injectable, OnDestroy, OnInit, signal } from "@angular/core";

// Exercise 022 — ngOnDestroy and DestroyRef (beginner).
// Goal:   let go of everything a component grabbed, so destroying it actually frees it.
// Drills: implements OnDestroy, undoing a subscription, DestroyRef.onDestroy() for
//         cleanup registered from a field initialiser, and proving no leak is left.
// Passes: when `npx jest exercises/01-beginner/ex022_lifecycle_ondestroy` is green.
//
// A leaked subscription is the classic Angular memory bug, and it is worse than "memory
// is wasted": the callback keeps firing on a component that is no longer on screen, so it
// writes to dead state and can throw from nowhere. Anything acquired in ngOnInit — a
// subscription, an interval, an event listener on window — has to be released here.
//
// DestroyRef is the newer, more composable half. Injecting it and calling onDestroy()
// registers cleanup *at the point you set the thing up*, which keeps the pair together
// instead of splitting them across two methods. It also works in places that have no
// lifecycle hooks at all, like a service or a helper function.

/** A stand-in for anything you subscribe to. Deliberately synchronous — no real timers. */
@Injectable({ providedIn: "root" })
export class Ticker {
  private readonly listeners = new Set<(tick: number) => void>();

  /** How many live listeners there are. Zero after a well-behaved component is gone. */
  listenerCount(): number {
    return this.listeners.size;
  }

  /** Register a listener and hand back the function that unregisters it. */
  subscribe(listener: (tick: number) => void): () => void {
    this.listeners.add(listener);
    return () => this.listeners.delete(listener);
  }

  /** Push a tick to everyone still listening. */
  emit(tick: number): void {
    for (const listener of this.listeners) {
      listener(tick);
    }
  }
}

// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="ticks">Ticks: {{ ticks() }}</p>
@Component({
  selector: "app-clock",
  standalone: true,
  template: `<p>TODO: render the clock — see the template contract above</p>`,
})
export class ClockComponent implements OnInit, OnDestroy {
  private readonly ticker = inject(Ticker);

  readonly ticks = signal(0);

  /** Which cleanups ran. Both "ngOnDestroy" and "destroyRef" must end up here. */
  readonly log: string[] = [];

  constructor() {
    // TODO: inject DestroyRef and register a cleanup that pushes "destroyRef" onto the
    // log. Note this happens during construction, long before anything is torn down.
    throw new Error("TODO: register the DestroyRef cleanup");
  }

  ngOnInit(): void {
    // TODO: subscribe to the ticker so each tick lands in `ticks`, and keep hold of the
    // unsubscribe function — there is no way to clean up later without it.
    throw new Error("TODO: implement ngOnInit");
  }

  ngOnDestroy(): void {
    // TODO: unsubscribe, then push "ngOnDestroy" onto the log.
    throw new Error("TODO: implement ngOnDestroy");
  }
}
