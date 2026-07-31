import { describe, expect, it } from "vitest";
import { useCounter } from "./useCounter";

describe("useCounter", () => {
  it("starts at the initial value", () => {
    const { count } = useCounter(5);
    expect(count.value).toBe(5);
  });

  it("increments and decrements", () => {
    const { count, increment, decrement } = useCounter();
    increment();
    increment();
    decrement();
    expect(count.value).toBe(1);
  });

  it("resets to the initial value", () => {
    const { count, increment, reset } = useCounter(3);
    increment();
    reset();
    expect(count.value).toBe(3);
  });
});
