import { createSignalStore } from "./signal-store";

interface CounterState {
  readonly count: number;
}

type CounterAction = { readonly type: "increment" } | { readonly type: "decrement" } | { readonly type: "reset" };

function counterReducer(state: CounterState, action: CounterAction): CounterState {
  switch (action.type) {
    case "increment":
      return { count: state.count + 1 };
    case "decrement":
      return { count: state.count - 1 };
    case "reset":
      return { count: 0 };
  }
}

interface TodoState {
  readonly items: readonly string[];
}

type TodoAction = { readonly type: "add"; readonly text: string } | { readonly type: "clear" };

function todoReducer(state: TodoState, action: TodoAction): TodoState {
  switch (action.type) {
    case "add":
      return { items: [...state.items, action.text] };
    case "clear":
      return { items: [] };
  }
}

describe("createSignalStore", () => {
  it("dispatches actions through the reducer and updates state", () => {
    const store = createSignalStore<CounterState, CounterAction>({ count: 0 }, counterReducer);

    expect(store.state().count).toBe(0);

    store.dispatch({ type: "increment" });
    store.dispatch({ type: "increment" });
    expect(store.state().count).toBe(2);

    store.dispatch({ type: "reset" });
    expect(store.state().count).toBe(0);
  });

  it("keeps two store instances of the same factory fully isolated from each other", () => {
    const storeA = createSignalStore<CounterState, CounterAction>({ count: 0 }, counterReducer);
    const storeB = createSignalStore<CounterState, CounterAction>({ count: 10 }, counterReducer);

    storeA.dispatch({ type: "increment" });
    storeA.dispatch({ type: "increment" });

    expect(storeA.state().count).toBe(2);
    expect(storeB.state().count).toBe(10); // untouched by storeA's dispatches
  });

  it("is generic — works with a completely different State/Action shape, not hardcoded to counters", () => {
    const store = createSignalStore<TodoState, TodoAction>({ items: [] }, todoReducer);

    store.dispatch({ type: "add", text: "milk" });
    store.dispatch({ type: "add", text: "eggs" });
    expect(store.state().items).toEqual(["milk", "eggs"]);

    store.dispatch({ type: "clear" });
    expect(store.state().items).toEqual([]);
  });

  it("exposes state as a genuinely read-only signal — no set/update escape hatch at runtime", () => {
    const store = createSignalStore<CounterState, CounterAction>({ count: 0 }, counterReducer);

    expect((store.state as unknown as { set?: unknown }).set).toBeUndefined();
    expect((store.state as unknown as { update?: unknown }).update).toBeUndefined();
  });

  it("does not mutate the initial state object handed to it", () => {
    const initial: CounterState = { count: 5 };
    const store = createSignalStore<CounterState, CounterAction>(initial, counterReducer);

    store.dispatch({ type: "increment" });

    expect(initial.count).toBe(5);
    expect(store.state().count).toBe(6);
  });
});
