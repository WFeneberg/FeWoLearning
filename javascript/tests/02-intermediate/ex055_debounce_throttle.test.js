import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { debounce, throttle } from "@ex/02-intermediate/ex055_debounce_throttle/index.js";

beforeEach(() => {
  vi.useFakeTimers();
});

afterEach(() => {
  vi.useRealTimers();
});

describe("ex055 debounce", () => {
  it("does not call before the delay", () => {
    const fn = vi.fn();
    debounce(fn, 100)();
    expect(fn).not.toHaveBeenCalled();
    vi.advanceTimersByTime(99);
    expect(fn).not.toHaveBeenCalled();
    vi.advanceTimersByTime(1);
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("restarts the clock on every call", () => {
    const fn = vi.fn();
    const debounced = debounce(fn, 100);
    debounced();
    vi.advanceTimersByTime(90);
    debounced();
    vi.advanceTimersByTime(90);
    expect(fn).not.toHaveBeenCalled();
    vi.advanceTimersByTime(10);
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("collapses a burst into one call with the LAST arguments", () => {
    const fn = vi.fn();
    const debounced = debounce(fn, 50);
    debounced("a");
    debounced("b");
    debounced("c");
    vi.advanceTimersByTime(50);
    expect(fn).toHaveBeenCalledTimes(1);
    expect(fn).toHaveBeenCalledWith("c");
  });

  it("can fire again after the window", () => {
    const fn = vi.fn();
    const debounced = debounce(fn, 50);
    debounced();
    vi.advanceTimersByTime(50);
    debounced();
    vi.advanceTimersByTime(50);
    expect(fn).toHaveBeenCalledTimes(2);
  });

  it("can be cancelled before it fires", () => {
    const fn = vi.fn();
    const debounced = debounce(fn, 50);
    debounced();
    debounced.cancel();
    vi.advanceTimersByTime(1000);
    expect(fn).not.toHaveBeenCalled();
  });
});

describe("ex055 throttle", () => {
  it("calls immediately — leading edge", () => {
    const fn = vi.fn();
    throttle(fn, 100)("first");
    expect(fn).toHaveBeenCalledWith("first");
  });

  it("swallows the calls inside the window", () => {
    const fn = vi.fn();
    const throttled = throttle(fn, 100);
    throttled(1);
    throttled(2);
    vi.advanceTimersByTime(99);
    throttled(3);
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("opens again once the window has passed", () => {
    const fn = vi.fn();
    const throttled = throttle(fn, 100);
    throttled(1);
    vi.advanceTimersByTime(100);
    throttled(2);
    expect(fn).toHaveBeenCalledTimes(2);
    expect(fn).toHaveBeenLastCalledWith(2);
  });

  it("does not queue a trailing call — a swallowed call is gone", () => {
    // The difference from debounce: this is a rate limit, not a delay.
    const fn = vi.fn();
    const throttled = throttle(fn, 100);
    throttled(1);
    throttled(2);
    vi.advanceTimersByTime(1000);
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("returns the function's result on a call that goes through", () => {
    const throttled = throttle((n) => n * 2, 100);
    expect(throttled(5)).toBe(10);
    expect(throttled(5)).toBeUndefined();
  });
});
