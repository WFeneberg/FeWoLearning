import {
  Component,
  DestroyRef,
  inject,
  Injectable,
  OnDestroy,
  OnInit,
  signal,
} from "@angular/core";

// Exercise 022 — ngOnDestroy and DestroyRef (reference solution).

@Injectable({ providedIn: "root" })
export class Ticker {
  private readonly listeners = new Set<(tick: number) => void>();

  listenerCount(): number {
    return this.listeners.size;
  }

  subscribe(listener: (tick: number) => void): () => void {
    this.listeners.add(listener);
    return () => this.listeners.delete(listener);
  }

  emit(tick: number): void {
    for (const listener of this.listeners) {
      listener(tick);
    }
  }
}

@Component({
  selector: "app-clock",
  standalone: true,
  template: `<p class="ticks">Ticks: {{ ticks() }}</p>`,
})
export class ClockComponent implements OnInit, OnDestroy {
  private readonly ticker = inject(Ticker);
  private readonly destroyRef = inject(DestroyRef);

  readonly ticks = signal(0);

  readonly log: string[] = [];

  /** Undefined until ngOnInit: there is nothing to unsubscribe from before then. */
  private stop?: () => void;

  constructor() {
    // Registered during construction but not run until teardown. The value of doing it
    // here is that setup and cleanup can sit side by side, even in code with no hooks.
    this.destroyRef.onDestroy(() => this.log.push("destroyRef"));
  }

  ngOnInit(): void {
    // Keeping the returned function is the whole game — without it there is no way back.
    this.stop = this.ticker.subscribe((tick) => this.ticks.set(tick));
  }

  ngOnDestroy(): void {
    this.stop?.();
    this.log.push("ngOnDestroy");
  }
}
