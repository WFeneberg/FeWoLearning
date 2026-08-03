import { describe, expect, it } from "vitest";
import { nextTick } from "vue";
import { useWatchSum } from "./useWatchSum";

describe("useWatchSum", () => {
  it("starts with no recorded sums", () => {
    const { sums } = useWatchSum(1, 2);
    expect(sums.value).toEqual([]);
  });

  it("records a sum when the first source changes", async () => {
    const { a, sums } = useWatchSum(1, 2);
    a.value = 5;
    await nextTick();
    expect(sums.value).toEqual([7]);
  });

  it("records a sum when the second source changes", async () => {
    const { b, sums } = useWatchSum(1, 2);
    b.value = 10;
    await nextTick();
    expect(sums.value).toEqual([11]);
  });

  it("accumulates sums across independent changes to each source", async () => {
    const { a, b, sums } = useWatchSum(1, 2);
    a.value = 5;
    await nextTick();
    b.value = 10;
    await nextTick();
    a.value = 20;
    await nextTick();
    expect(sums.value).toEqual([7, 15, 30]);
  });
});
