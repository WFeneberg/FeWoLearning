import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { useDebouncedRef } from "./useDebouncedRef";

describe("useDebouncedRef", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("starts at the initial value", () => {
    const { value } = useDebouncedRef("start", 200);
    expect(value.value).toBe("start");
  });

  it("does not update before the delay elapses", () => {
    const { value, set } = useDebouncedRef(0, 200);
    set(1);
    vi.advanceTimersByTime(199);
    expect(value.value).toBe(0);
  });

  it("updates to the set value once the delay elapses", () => {
    const { value, set } = useDebouncedRef(0, 200);
    set(1);
    vi.advanceTimersByTime(200);
    expect(value.value).toBe(1);
  });

  it("settles to only the final value after rapid consecutive sets", () => {
    const { value, set } = useDebouncedRef(0, 200);
    set(1);
    vi.advanceTimersByTime(100);
    set(2);
    vi.advanceTimersByTime(100);
    set(3);
    vi.advanceTimersByTime(100);
    // Still within 200ms of the last set — no update yet.
    expect(value.value).toBe(0);
    vi.advanceTimersByTime(100);
    // 200ms after the final set(3): only the last value applied.
    expect(value.value).toBe(3);
  });

  it("resets the timer on each new set call", () => {
    const { value, set } = useDebouncedRef("a", 200);
    set("b");
    vi.advanceTimersByTime(150);
    set("c");
    vi.advanceTimersByTime(150);
    // 300ms total elapsed, but only 150ms since the last set("c").
    expect(value.value).toBe("a");
    vi.advanceTimersByTime(50);
    expect(value.value).toBe("c");
  });
});
