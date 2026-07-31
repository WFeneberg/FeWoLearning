import { describe, expect, it } from "vitest";
import { useFetch } from "./useFetch";

function flushPromises() {
  return new Promise((resolve) => setTimeout(resolve, 0));
}

describe("useFetch", () => {
  it("starts in a loading state with no data or error", () => {
    const { data, error, loading } = useFetch(() => Promise.resolve("value"));
    expect(loading.value).toBe(true);
    expect(data.value).toBeNull();
    expect(error.value).toBeNull();
  });

  it("sets data and clears loading once the fetcher resolves", async () => {
    const { data, error, loading } = useFetch(() => Promise.resolve({ id: 1, name: "Ada" }));
    await flushPromises();
    expect(loading.value).toBe(false);
    expect(data.value).toEqual({ id: 1, name: "Ada" });
    expect(error.value).toBeNull();
  });

  it("sets error and clears loading once the fetcher rejects", async () => {
    const failure = new Error("network down");
    const { data, error, loading } = useFetch(() => Promise.reject(failure));
    await flushPromises();
    expect(loading.value).toBe(false);
    expect(data.value).toBeNull();
    expect(error.value).toBe(failure);
  });
});
