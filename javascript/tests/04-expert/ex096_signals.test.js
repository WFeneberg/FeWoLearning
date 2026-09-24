import { describe, expect, it, vi } from "vitest";
import { batch, computed, effect, signal } from "@ex/04-expert/ex096_signals/index.js";

describe("ex096 signal", () => {
  it("reads and writes", () => {
    const count = signal(1);
    expect(count.get()).toBe(1);
    count.set(2);
    expect(count.get()).toBe(2);
  });

  it("peeks without tracking", () => {
    const count = signal(1);
    const runs = vi.fn();
    effect(() => {
      runs(count.peek());
    });
    count.set(2);
    expect(runs).toHaveBeenCalledTimes(1);
  });
});

describe("ex096 computed", () => {
  it("derives", () => {
    const first = signal(2);
    const second = signal(3);
    const sum = computed(() => first.get() + second.get());
    expect(sum.get()).toBe(5);
    first.set(10);
    expect(sum.get()).toBe(13);
  });

  it("is lazy: nothing runs until it is read", () => {
    const fn = vi.fn(() => 1);
    computed(fn);
    expect(fn).not.toHaveBeenCalled();
  });

  it("caches until a dependency changes", () => {
    const count = signal(1);
    const fn = vi.fn(() => count.get() * 2);
    const doubled = computed(fn);
    doubled.get();
    doubled.get();
    doubled.get();
    expect(fn).toHaveBeenCalledTimes(1);
    count.set(2);
    expect(doubled.get()).toBe(4);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("does not recompute for an unrelated signal", () => {
    const tracked = signal(1);
    const unrelated = signal("x");
    const fn = vi.fn(() => tracked.get());
    const derived = computed(fn);
    derived.get();
    unrelated.set("y");
    derived.get();
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("does not recompute for a write of the same value", () => {
    const count = signal(1);
    const fn = vi.fn(() => count.get());
    const derived = computed(fn);
    derived.get();
    count.set(1);
    derived.get();
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("chains", () => {
    const count = signal(1);
    const doubled = computed(() => count.get() * 2);
    const quadrupled = computed(() => doubled.get() * 2);
    expect(quadrupled.get()).toBe(4);
    count.set(3);
    expect(quadrupled.get()).toBe(12);
  });
});

describe("ex096 effect", () => {
  it("runs immediately and on every change", () => {
    const count = signal(1);
    const seen = [];
    effect(() => seen.push(count.get()));
    count.set(2);
    count.set(3);
    expect(seen).toEqual([1, 2, 3]);
  });

  it("tracks a computed it reads", () => {
    const count = signal(1);
    const doubled = computed(() => count.get() * 2);
    const seen = [];
    effect(() => seen.push(doubled.get()));
    count.set(5);
    expect(seen).toEqual([2, 10]);
  });

  it("stops when disposed", () => {
    const count = signal(1);
    const runs = vi.fn();
    const stop = effect(() => runs(count.get()));
    stop();
    count.set(2);
    expect(runs).toHaveBeenCalledTimes(1);
  });

  it("keeps two effects independent", () => {
    const count = signal(1);
    const a = vi.fn();
    const b = vi.fn();
    const stopA = effect(() => a(count.get()));
    effect(() => b(count.get()));
    stopA();
    count.set(2);
    expect(a).toHaveBeenCalledTimes(1);
    expect(b).toHaveBeenCalledTimes(2);
  });
});

describe("ex096 batch", () => {
  it("collapses several writes into one effect run", () => {
    const first = signal(1);
    const second = signal(1);
    const runs = vi.fn();
    effect(() => runs(first.get() + second.get()));
    batch(() => {
      first.set(10);
      second.set(20);
    });
    expect(runs).toHaveBeenCalledTimes(2);
    expect(runs).toHaveBeenLastCalledWith(30);
  });

  it("returns the callback's result", () => {
    expect(batch(() => "result")).toBe("result");
  });

  it("flushes even when the callback throws", () => {
    const count = signal(1);
    const runs = vi.fn();
    effect(() => runs(count.get()));
    expect(() =>
      batch(() => {
        count.set(2);
        throw new Error("mid-batch");
      }),
    ).toThrow("mid-batch");
    expect(runs).toHaveBeenCalledTimes(2);
  });

  it("nests", () => {
    const count = signal(0);
    const runs = vi.fn();
    effect(() => runs(count.get()));
    batch(() => {
      count.set(1);
      batch(() => count.set(2));
      count.set(3);
    });
    expect(runs).toHaveBeenCalledTimes(2);
    expect(runs).toHaveBeenLastCalledWith(3);
  });
});
