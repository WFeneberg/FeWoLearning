import { TestBed } from "@angular/core/testing";
import { AuditStore } from "./audit-store";

describe("AuditStore", () => {
  let store: AuditStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(AuditStore);
  });

  it("summarises the amount", () => {
    expect(store.summary()).toBe("amount: 100");
  });

  it("follows the signal it genuinely depends on", () => {
    expect(store.summary()).toBe("amount: 100");

    store.amount.set(250);

    expect(store.summary()).toBe("amount: 250");
    expect(store.recomputes).toBe(2);
  });

  it("reads the untracked value correctly, not stalely", () => {
    store.noteRead();
    store.noteRead();
    store.verbose.set(true);

    // untracked() does not mean cached — the value is current when it is read.
    expect(store.summary()).toBe("amount: 100 (read 2 times)");
  });

  it("does not re-run when an untracked signal changes", () => {
    expect(store.summary()).toBe("amount: 100");
    const before = store.recomputes;

    store.noteRead();
    store.noteRead();
    store.noteRead();

    expect(store.summary()).toBe("amount: 100");
    expect(store.recomputes).toBe(before);
  });

  it("does not re-run when the peeked-at flag changes either", () => {
    expect(store.summary()).toBe("amount: 100");
    const before = store.recomputes;

    store.verbose.set(true);

    // The flag really did change, and the cached summary does not reflect it. That is the
    // cost of an untracked read, and why it must only be used for incidental values.
    expect(store.summary()).toBe("amount: 100");
    expect(store.recomputes).toBe(before);
  });

  it("picks the untracked values up on the next real recompute", () => {
    expect(store.summary()).toBe("amount: 100");
    store.verbose.set(true);
    store.noteRead();
    expect(store.summary()).toBe("amount: 100");

    store.amount.set(7);

    // A change to a *tracked* signal re-runs the body, which re-reads everything.
    expect(store.summary()).toBe("amount: 7 (read 1 times)");
  });

  it("re-runs the eager version for any of the three", () => {
    expect(store.eagerSummary()).toBe("amount: 100");
    const before = store.eagerRecomputes;

    store.noteRead();

    // Subscribed to `reads`, so a bump it does not care about costs it a recompute.
    expect(store.eagerSummary()).toBe("amount: 100");
    expect(store.eagerRecomputes).toBe(before + 1);
  });

  it("keeps the two versions in agreement once both are fresh", () => {
    store.verbose.set(true);
    store.noteRead();

    expect(store.eagerSummary()).toBe("amount: 100 (read 1 times)");
    expect(store.summary()).toBe("amount: 100 (read 1 times)");
  });

  it("costs the eager version a recompute where the untracked one pays nothing", () => {
    expect(store.summary()).toBe("amount: 100");
    expect(store.eagerSummary()).toBe("amount: 100");
    const lazyBefore = store.recomputes;
    const eagerBefore = store.eagerRecomputes;

    store.noteRead();
    store.noteRead();
    store.eagerSummary();
    store.summary();

    expect(store.eagerRecomputes).toBeGreaterThan(eagerBefore);
    expect(store.recomputes).toBe(lazyBefore);
  });

  it("reads through the helper without subscribing", () => {
    store.noteRead();

    expect(store.currentReadsUntracked()).toBe(1);
  });

  it("protects a computed that only calls the helper", () => {
    expect(store.doubledReads()).toBe(0);

    store.noteRead();
    store.noteRead();

    // The helper wraps its own read, so the caller never subscribed — untracked() is not
    // limited to being written inline in the computed.
    expect(store.doubledReads()).toBe(0);
  });

  it("counts reads regardless of who is watching", () => {
    store.noteRead();
    store.noteRead();
    store.noteRead();

    expect(store.reads()).toBe(3);
  });
});
