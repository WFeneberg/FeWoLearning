import { describe, expect, it } from "vitest";
import { useStorage, type StorageLike } from "./useStorage";

function createMockStore(initial: Record<string, string> = {}): StorageLike {
  const data: Record<string, string> = { ...initial };
  return {
    getItem: (key: string) => (key in data ? data[key] : null),
    setItem: (key: string, value: string) => {
      data[key] = value;
    },
  };
}

describe("useStorage", () => {
  it("initializes from the default value when the key is empty", () => {
    const store = createMockStore();
    const value = useStorage("theme", "light", store);
    expect(value.value).toBe("light");
  });

  it("initializes from a pre-populated key in the store", () => {
    const store = createMockStore({ theme: '"dark"' });
    const value = useStorage("theme", "light", store);
    expect(value.value).toBe("dark");
  });

  it("writes updates to the store", () => {
    const store = createMockStore();
    const value = useStorage("theme", "light", store);
    value.value = "dark";
    expect(store.getItem("theme")).toBe('"dark"');
  });
});
