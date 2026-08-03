import { describe, expect, it, vi } from "vitest";
import { defineComponent, h, isReactive, nextTick } from "vue";
import { mount } from "@vue/test-utils";
import { defineStoreModule } from "./useTypedStoreModule";

interface CartState {
  items: Array<{ sku: string; qty: number }>;
  discount: number;
}

const isSku = (p: unknown): p is { sku: string; qty: number } =>
  typeof p === "object" &&
  p !== null &&
  typeof (p as { sku?: unknown }).sku === "string" &&
  typeof (p as { qty?: unknown }).qty === "number";

const isNumber = (p: unknown): p is number => typeof p === "number";

/** A fresh module definition per test, so the singleton never leaks across tests. */
function makeCartModule() {
  return defineStoreModule({
    state: (): CartState => ({ items: [], discount: 0 }),
    getters: {
      count: (state) => state.items.reduce((sum, i) => sum + i.qty, 0),
      isEmpty: (state) => state.items.length === 0,
    },
    actions: {
      add: {
        guard: isSku,
        handler: (state, payload) => {
          state.items.push(payload);
        },
      },
      setDiscount: {
        guard: isNumber,
        handler: (state, payload) => {
          state.discount = payload;
        },
      },
    },
  });
}

describe("defineStoreModule", () => {
  it("exposes reactive state seeded from the state factory", () => {
    const useCart = makeCartModule();
    const { state } = useCart();

    expect(isReactive(state)).toBe(true);
    expect(state.items).toEqual([]);
    expect(state.discount).toBe(0);
  });

  it("exposes getters as computed refs derived from state", () => {
    const useCart = makeCartModule();
    const { state, getters } = useCart();

    expect(getters.count.value).toBe(0);
    expect(getters.isEmpty.value).toBe(true);

    state.items.push({ sku: "A", qty: 3 });

    // Recomputed from the mutated state, not a stale snapshot.
    expect(getters.count.value).toBe(3);
    expect(getters.isEmpty.value).toBe(false);
  });

  it("runs an action's handler against the store's own state", () => {
    const useCart = makeCartModule();
    const { state, getters, actions } = useCart();

    actions.add({ sku: "B", qty: 2 });
    actions.setDiscount(15);

    expect(state.items).toEqual([{ sku: "B", qty: 2 }]);
    expect(state.discount).toBe(15);
    expect(getters.count.value).toBe(2);
  });

  it("rejects a payload that fails the action's guard, leaving state untouched", () => {
    const useCart = makeCartModule();
    const { state, actions } = useCart();

    // `as never` defeats the static payload type on purpose: the guard is the
    // runtime half of the contract and must reject what the compiler would.
    expect(() => actions.add("not-a-sku" as never)).toThrow(TypeError);
    expect(() => actions.setDiscount({} as never)).toThrow(/setDiscount/);

    expect(state.items).toEqual([]);
    expect(state.discount).toBe(0);
  });

  it("memoizes one instance per definition, so every consumer shares state", () => {
    const useCart = makeCartModule();
    const first = useCart();
    const second = useCart();

    expect(second.state).toBe(first.state);
    expect(second.getters.count).toBe(first.getters.count);

    first.actions.add({ sku: "C", qty: 4 });
    expect(second.getters.count.value).toBe(4);
  });

  it("keeps two separate definitions fully isolated", () => {
    const useCartA = makeCartModule();
    const useCartB = makeCartModule();

    useCartA().actions.add({ sku: "D", qty: 7 });

    expect(useCartA().getters.count.value).toBe(7);
    expect(useCartB().getters.count.value).toBe(0);
    expect(useCartB().state.items).toEqual([]);
  });

  it("calls the state factory lazily — only on the first useStore() call", () => {
    const factory = vi.fn((): CartState => ({ items: [], discount: 0 }));
    const useCart = defineStoreModule({
      state: factory,
      getters: {},
      actions: {},
    });

    expect(factory).not.toHaveBeenCalled();

    useCart();
    useCart();

    expect(factory).toHaveBeenCalledTimes(1);
  });

  it("drives a component re-render when an action mutates shared state", async () => {
    const useCart = makeCartModule();
    const Cart = defineComponent({
      setup() {
        const { getters } = useCart();
        return () => h("span", `count:${getters.count.value}`);
      },
    });

    const wrapper = mount(Cart);
    expect(wrapper.text()).toBe("count:0");

    useCart().actions.add({ sku: "E", qty: 5 });
    await nextTick();

    expect(wrapper.text()).toBe("count:5");
  });
});
