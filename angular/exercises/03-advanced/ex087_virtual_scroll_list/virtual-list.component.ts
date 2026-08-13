import { Component, computed, input, signal } from "@angular/core";

// Exercise 087 — virtual scroll: windowed rendering over a large list (advanced).
// Goal:   render only the handful of rows actually visible in a scrolled viewport, not the whole
//         list, while keeping the scrollbar's size and position accurate.
// Drills: a pure windowing calculation (index range + spacer heights from scroll position), and
//         wiring it to a real `(scroll)` event without any external virtual-scroll library.
// Passes: when `npx jest exercises/03-advanced/ex087_virtual_scroll_list` is green.
//
// This project has no CDK dependency, so "virtual scrolling" here means exactly what the phrase
// describes and nothing more: a fixed-height row list, a viewport shorter than the full list, and
// arithmetic that converts `scrollTop` into "which rows are visible right now." Two empty `<div>`s
// above and below the rendered rows — the spacers — stand in for the rows that are *not* rendered,
// giving the browser's real scrollbar the same total height and position it would have if every row
// were actually in the DOM. Get the spacer heights wrong and the scrollbar jumps or the content
// shifts; that arithmetic is the entire exercise.
//
// `overscan` renders a few extra rows beyond what is strictly visible, on both ends of the window.
// Without it, a fast scroll can outrun rendering and flash empty space for a frame; a couple of
// spare rows already in the DOM absorb that. `computeVirtualWindow` is intentionally a plain,
// synchronous, dependency-free function — it is the thing this exercise's spec tests directly, with
// exact numbers, because it is far easier (and far more precise) to assert "row 9 through row 16"
// against arithmetic than to simulate real scroll physics inside jsdom, which has none.
//
// The component only wires that pure function to reactive state: `scrollTop` is a signal written by
// the one DOM event this component listens to, `virtualWindow` is a computed projection of it (plus
// the inputs), and `visibleItems` slices the full list down to just the window. Nothing here ever
// creates or destroys a DOM node for a row outside that slice.

export interface VirtualWindowInput {
  readonly scrollTop: number;
  readonly viewportHeight: number;
  readonly itemHeight: number;
  readonly totalItems: number;
  readonly overscan?: number;
}

export interface VirtualWindow {
  readonly startIndex: number;
  readonly endIndex: number; // exclusive
  readonly topSpacerHeight: number;
  readonly bottomSpacerHeight: number;
}

/**
 * TODO: implement computeVirtualWindow — a pure function from scroll position to the visible index
 * range and spacer heights.
 *   - `overscan` defaults to 2 when not provided.
 *   - `firstVisible` = floor(scrollTop / itemHeight); `visibleCount` = ceil(viewportHeight / itemHeight).
 *   - `startIndex` = max(0, firstVisible - overscan).
 *   - `endIndex` = min(totalItems, firstVisible + visibleCount + overscan). Never let it go below
 *     `startIndex` (an empty list should yield startIndex === endIndex === 0).
 *   - `topSpacerHeight` = startIndex * itemHeight.
 *   - `bottomSpacerHeight` = (totalItems - endIndex) * itemHeight.
 */
export function computeVirtualWindow(input: VirtualWindowInput): VirtualWindow {
  throw new Error("TODO: implement computeVirtualWindow");
}

@Component({
  selector: "app-virtual-list",
  standalone: true,
  template: `
    <div class="viewport" [style.height.px]="viewportHeight()" (scroll)="onScroll($event)">
      <div class="top-spacer" [style.height.px]="virtualWindow().topSpacerHeight"></div>
      @for (item of visibleItems(); track item) {
        <div class="row" [style.height.px]="itemHeight()">{{ item }}</div>
      }
      <div class="bottom-spacer" [style.height.px]="virtualWindow().bottomSpacerHeight"></div>
    </div>
  `,
})
export class VirtualListComponent {
  readonly items = input.required<readonly string[]>();
  readonly itemHeight = input(32);
  readonly viewportHeight = input(320);
  readonly overscan = input(2);

  readonly scrollTop = signal(0);

  readonly virtualWindow = computed<VirtualWindow>(() =>
    computeVirtualWindow({
      scrollTop: this.scrollTop(),
      viewportHeight: this.viewportHeight(),
      itemHeight: this.itemHeight(),
      totalItems: this.items().length,
      overscan: this.overscan(),
    }),
  );

  readonly visibleItems = computed(() => {
    const { startIndex, endIndex } = this.virtualWindow();
    return this.items().slice(startIndex, endIndex);
  });

  /**
   * TODO: implement onScroll — read `scrollTop` off `event.target` (an HTMLElement) and write it
   * into the `scrollTop` signal above. That single write is what makes `virtualWindow` and
   * `visibleItems` recompute.
   */
  onScroll(event: Event): void {
    throw new Error("TODO: implement onScroll");
  }
}
