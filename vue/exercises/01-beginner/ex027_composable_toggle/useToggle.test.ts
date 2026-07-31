import { describe, expect, it } from "vitest";
import { useToggle } from "./useToggle";

describe("useToggle", () => {
  it("starts at the initial value", () => {
    const [state] = useToggle(true);
    expect(state.value).toBe(true);
  });

  it("defaults to false when no initial value is given", () => {
    const [state] = useToggle();
    expect(state.value).toBe(false);
  });

  it("flips the boolean state each time toggle is invoked", () => {
    const [state, toggle] = useToggle(false);
    toggle();
    expect(state.value).toBe(true);
    toggle();
    expect(state.value).toBe(false);
    toggle();
    expect(state.value).toBe(true);
  });
});
