import { computed, Injectable, signal, untracked } from "@angular/core";

// Exercise 033 — untracked() (reference solution).

@Injectable({ providedIn: "root" })
export class AuditStore {
  readonly amount = signal(100);

  readonly reads = signal(0);

  readonly verbose = signal(false);

  recomputes = 0;

  readonly summary = computed<string>(() => {
    this.recomputes += 1;
    // Tracked: this is what the result genuinely depends on.
    const amount = this.amount();
    // Untracked: read now, current value, no subscription. Neither of these can trigger a
    // re-run, which is the whole point — and also why the cached string can lag behind them.
    const verbose = untracked(() => this.verbose());
    const reads = untracked(() => this.reads());
    return verbose ? `amount: ${amount} (read ${reads} times)` : `amount: ${amount}`;
  });

  readonly eagerSummary = computed<string>(() => {
    this.eagerRecomputes += 1;
    // Every read is tracked, so a bump to `reads` costs a recompute it gains nothing from.
    const amount = this.amount();
    const verbose = this.verbose();
    const reads = this.reads();
    return verbose ? `amount: ${amount} (read ${reads} times)` : `amount: ${amount}`;
  });

  eagerRecomputes = 0;

  noteRead(): void {
    this.reads.update((n) => n + 1);
  }

  currentReadsUntracked(): number {
    // Wrapping it here protects every caller, rather than each one having to remember.
    return untracked(() => this.reads());
  }

  readonly doubledReads = computed<number>(() => {
    // No untracked() visible at this level, yet no dependency is created — untracked works
    // by the call stack it runs in, not by where it is written.
    return this.currentReadsUntracked() * 2;
  });
}
