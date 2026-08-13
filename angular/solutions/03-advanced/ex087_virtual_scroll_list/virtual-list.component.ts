import { Component, computed, input, signal } from "@angular/core";

// Exercise 087 — virtual scroll: windowed rendering over a large list (reference solution).

export interface VirtualWindowInput {
  readonly scrollTop: number;
  readonly viewportHeight: number;
  readonly itemHeight: number;
  readonly totalItems: number;
  readonly overscan?: number;
}

export interface VirtualWindow {
  readonly startIndex: number;
  readonly endIndex: number;
  readonly topSpacerHeight: number;
  readonly bottomSpacerHeight: number;
}

export function computeVirtualWindow(input: VirtualWindowInput): VirtualWindow {
  const overscan = input.overscan ?? 2;
  const firstVisible = Math.floor(input.scrollTop / input.itemHeight);
  const visibleCount = Math.ceil(input.viewportHeight / input.itemHeight);

  const startIndex = Math.max(0, firstVisible - overscan);
  const endIndex = Math.max(startIndex, Math.min(input.totalItems, firstVisible + visibleCount + overscan));

  return {
    startIndex,
    endIndex,
    topSpacerHeight: startIndex * input.itemHeight,
    bottomSpacerHeight: (input.totalItems - endIndex) * input.itemHeight,
  };
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

  onScroll(event: Event): void {
    this.scrollTop.set((event.target as HTMLElement).scrollTop);
  }
}
