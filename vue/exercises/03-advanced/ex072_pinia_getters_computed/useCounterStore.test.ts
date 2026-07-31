import { describe, expect, it, vi } from "vitest";
import { useCounterStore } from "./useCounterStore";

describe("useCounterStore", () => {
  it("derives doubleCount from the initial count", () => {
    const { count, doubleCount } = useCounterStore(3);
    expect(count.value).toBe(3);
    expect(doubleCount.value).toBe(6);
  });

  it("recomputes doubleCount reactively after the increment action", () => {
    const { count, doubleCount, increment } = useCounterStore(2);
    expect(doubleCount.value).toBe(4);

    increment();
    expect(count.value).toBe(3);
    expect(doubleCount.value).toBe(6);

    increment(4);
    expect(count.value).toBe(7);
    expect(doubleCount.value).toBe(14);
  });

  it("chains quadrupleCount off of doubleCount, staying in sync", () => {
    const { quadrupleCount, increment } = useCounterStore(1);
    expect(quadrupleCount.value).toBe(4);

    increment(2);
    expect(quadrupleCount.value).toBe(12);
  });

  it("getters are read-only computed refs, ignoring direct writes", () => {
    const warn = vi.spyOn(console, "warn").mockImplementation(() => {});
    const { doubleCount, increment } = useCounterStore(5);

    // @ts-expect-error getters must not be assignable
    doubleCount.value = 999;
    expect(warn).toHaveBeenCalled();
    // the write is a no-op: the getter still reflects derived state
    expect(doubleCount.value).toBe(10);

    increment();
    expect(doubleCount.value).toBe(12);

    warn.mockRestore();
  });
});
