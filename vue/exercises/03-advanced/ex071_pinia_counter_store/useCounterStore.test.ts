import { beforeEach, describe, expect, it } from "vitest";
import { createPinia, setActivePinia, useCounterStore } from "./useCounterStore";

describe("useCounterStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("starts at count 0 on a fresh pinia", () => {
    const store = useCounterStore();
    expect(store.count).toBe(0);
  });

  it("increments count by 1", () => {
    const store = useCounterStore();
    store.increment();
    expect(store.count).toBe(1);
  });

  it("increments repeatedly", () => {
    const store = useCounterStore();
    store.increment();
    store.increment();
    store.increment();
    expect(store.count).toBe(3);
  });

  it("shares state across calls within the same active pinia", () => {
    const storeA = useCounterStore();
    const storeB = useCounterStore();
    storeA.increment();
    expect(storeB.count).toBe(1);
  });

  it("isolates state across different pinia instances", () => {
    const storeA = useCounterStore();
    storeA.increment();
    expect(storeA.count).toBe(1);

    setActivePinia(createPinia());
    const storeB = useCounterStore();
    expect(storeB.count).toBe(0);
  });

  it("registers the store under the active pinia's registry", () => {
    const pinia = createPinia();
    setActivePinia(pinia);
    useCounterStore();
    expect(pinia.stores.has("counter")).toBe(true);
  });
});
