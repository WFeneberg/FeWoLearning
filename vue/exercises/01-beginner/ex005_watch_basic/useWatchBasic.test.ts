import { describe, expect, it, nextTick } from "vitest";
import { useWatchBasic } from "./useWatchBasic";

describe("useWatchBasic", () => {
  it("starts with an empty history", () => {
    const { count, history } = useWatchBasic(0);
    expect(count.value).toBe(0);
    expect(history).toEqual([]);
  });

  it("records old/new value pairs after updates", async () => {
    const { count, history } = useWatchBasic(0);

    count.value = 1;
    await nextTick();
    count.value = 5;
    await nextTick();

    expect(history).toEqual([
      [0, 1],
      [1, 5],
    ]);
  });
});
