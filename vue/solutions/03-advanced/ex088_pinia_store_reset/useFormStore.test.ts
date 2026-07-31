import { describe, expect, it } from "vitest";
import { useFormStore } from "./useFormStore";

describe("useFormStore", () => {
  it("starts with the built-in defaults when no overrides are given", () => {
    const store = useFormStore();
    expect(store.firstName).toBe("");
    expect(store.lastName).toBe("");
    expect(store.email).toBe("");
    expect(store.subscribed).toBe(false);
  });

  it("seeds initial state from the given overrides", () => {
    const store = useFormStore({ firstName: "Ada", subscribed: true });
    expect(store.firstName).toBe("Ada");
    expect(store.lastName).toBe("");
    expect(store.subscribed).toBe(true);
  });

  it("$patch updates several fields at once, leaving the rest untouched", () => {
    const store = useFormStore({ firstName: "Ada" });
    store.$patch({ lastName: "Lovelace", email: "ada@example.com" });

    expect(store.firstName).toBe("Ada");
    expect(store.lastName).toBe("Lovelace");
    expect(store.email).toBe("ada@example.com");
    expect(store.subscribed).toBe(false);
  });

  it("$patch can be called repeatedly, each call layering on top of the last", () => {
    const store = useFormStore();
    store.$patch({ firstName: "Grace" });
    store.$patch({ lastName: "Hopper", subscribed: true });

    expect(store.firstName).toBe("Grace");
    expect(store.lastName).toBe("Hopper");
    expect(store.subscribed).toBe(true);
  });

  it("$reset restores the initial snapshot after patches, not hard-coded defaults", () => {
    const store = useFormStore({ firstName: "Ada", email: "ada@example.com" });
    store.$patch({ lastName: "Lovelace", subscribed: true, firstName: "Changed" });

    store.$reset();

    expect(store.firstName).toBe("Ada");
    expect(store.lastName).toBe("");
    expect(store.email).toBe("ada@example.com");
    expect(store.subscribed).toBe(false);
  });

  it("resetForm behaves exactly like $reset", () => {
    const store = useFormStore({ subscribed: true });
    store.$patch({ firstName: "Katherine", lastName: "Johnson", subscribed: false });

    store.resetForm();

    expect(store.firstName).toBe("");
    expect(store.lastName).toBe("");
    expect(store.subscribed).toBe(true);
  });

  it("resetting one store instance never affects a separately created one", () => {
    const storeA = useFormStore({ firstName: "A" });
    const storeB = useFormStore({ firstName: "B" });

    storeA.$patch({ firstName: "Changed A" });
    storeA.resetForm();

    expect(storeA.firstName).toBe("A");
    expect(storeB.firstName).toBe("B");
  });

  it("state can be patched again and re-reset after a previous reset", () => {
    const store = useFormStore({ firstName: "Ada" });
    store.$patch({ firstName: "First change" });
    store.$reset();
    expect(store.firstName).toBe("Ada");

    store.$patch({ firstName: "Second change", lastName: "Lovelace" });
    expect(store.firstName).toBe("Second change");
    expect(store.lastName).toBe("Lovelace");

    store.$reset();
    expect(store.firstName).toBe("Ada");
    expect(store.lastName).toBe("");
  });
});
