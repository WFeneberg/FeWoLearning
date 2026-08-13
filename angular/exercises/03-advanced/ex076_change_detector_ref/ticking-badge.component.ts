import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject } from "@angular/core";

// Exercise 076 — ChangeDetectorRef: markForCheck, detach, and manual detection (advanced).
// Goal:   drive a component's rendering entirely by hand, for state that changes outside anything
//         Angular can see on its own.
// Drills: ChangeDetectorRef.markForCheck(), .detach(), and .detectChanges() — the three manual
//         levers OnPush leaves available once nothing else applies.
// Passes: when `npx jest exercises/03-advanced/ex076_change_detector_ref` is green.
//
// This component deliberately does *not* use a signal for its state. Signals already solve the
// common version of this problem — a template that reads a signal gets marked for re-check the
// moment that signal is written, no manual bookkeeping required. ChangeDetectorRef is what is left
// over for the case signals do not cover: state that changes from *outside* Angular's knowledge
// entirely — a raw `setInterval`, a WebSocket message handler, a third-party library's callback —
// where nothing in the framework observed the mutation happening.
//
// `markForCheck()` does not render anything by itself. It flags this view (and walks up marking
// every ancestor) as "worth looking at" the next time change detection runs, so that whichever
// mechanism actually triggers a pass will not skip an OnPush component just because none of its
// inputs changed. On its own it does nothing until something runs a pass — in this exercise, that
// something is an explicit `fixture.detectChanges()` call standing in for a real app's zone tick.
//
// `detach()` is stronger: it removes the view from that tree walk altogether. Once detached,
// nothing outside this component — not an ancestor's detectChanges(), not markForCheck() called on
// this same ref — will cause it to render, because the walk never reaches it. The only thing that
// still works is calling `detectChanges()` directly on *this* view's own ref: that is a direct
// instruction to refresh this exact view right now, bypassing the tree walk (and its dirty-check
// gate) entirely — which is why it is the one operation that still functions after detach().

@Component({
  selector: "app-ticking-badge",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="badge">{{ label }}</span>`,
})
export class TickingBadgeComponent {
  private readonly cdr = inject(ChangeDetectorRef);

  /** A plain field, not a signal — deliberately outside Angular's own reactivity graph. */
  label = "0";

  /**
   * TODO: set `label`, then tell Angular this view might need checking on the next pass. Without
   * the second half, the field changes but OnPush has no reason to know, so nothing renders.
   */
  setLabel(value: string): void {
    throw new Error("TODO: implement setLabel");
  }

  /**
   * TODO: stop this view from being included in change detection at all — not even markForCheck()
   * calls made afterwards will bring it back into the walk.
   */
  pause(): void {
    throw new Error("TODO: implement pause");
  }

  /** TODO: force this exact view to render right now, regardless of pause(). */
  renderNow(): void {
    throw new Error("TODO: implement renderNow");
  }
}
