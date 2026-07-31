import { describe, expect, it } from "vitest";
import { useDoubleRef } from "./useDoubleRef";

describe("useDoubleRef", () => {
  it("doubles the initial value", () => {
    const { double } = useDoubleRef(3);
    expect(double()).toBe(6);
  });

  it("doubles the current value after setting count.value", () => {
    const { count, double } = useDoubleRef();
    count.value = 5;
    expect(double()).toBe(10);
  });

  it("reflects further updates to count", () => {
    const { count, double } = useDoubleRef(1);
    count.value = 7;
    expect(double()).toBe(14);
  });
});
