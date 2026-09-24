import { describe, expect, it } from "vitest";
import { reactive, toRaw } from "@ex/04-expert/ex092_deep_reactive_proxy/index.js";

const track = (target) => {
  const changes = [];
  return { state: reactive(target, (change) => changes.push(change)), changes };
};

describe("ex092 reactive — writes", () => {
  it("reports a top-level write", () => {
    const { state, changes } = track({ count: 0 });
    state.count = 1;
    expect(changes).toEqual([{ path: ["count"], value: 1, previous: 0 }]);
  });

  it("reports the full path for a nested write", () => {
    const { state, changes } = track({ user: { address: { city: "Bern" } } });
    state.user.address.city = "Zurich";
    expect(changes[0].path).toEqual(["user", "address", "city"]);
    expect(changes[0].value).toBe("Zurich");
    expect(changes[0].previous).toBe("Bern");
  });

  it("really updates the underlying object", () => {
    const target = { user: { name: "ada" } };
    const state = reactive(target, () => {});
    state.user.name = "grace";
    expect(target.user.name).toBe("grace");
  });

  it("notifies nothing for a write of the same value", () => {
    const { state, changes } = track({ count: 1, flag: NaN });
    state.count = 1;
    state.flag = NaN;
    expect(changes).toEqual([]);
  });

  it("reports a new key", () => {
    const { state, changes } = track({});
    state.fresh = 1;
    expect(changes).toEqual([{ path: ["fresh"], value: 1, previous: undefined }]);
  });

  it("reports a delete", () => {
    const { state, changes } = track({ gone: 1 });
    delete state.gone;
    expect(changes).toEqual([{ path: ["gone"], value: undefined, previous: 1 }]);
    expect(changes).toHaveLength(1);
  });

  it("reports nothing for deleting a key that was not there", () => {
    const { state, changes } = track({});
    delete state.nothing;
    expect(changes).toEqual([]);
  });
});

describe("ex092 reactive — arrays", () => {
  it("reports an index write", () => {
    const { state, changes } = track({ list: [1, 2] });
    state.list[0] = 9;
    expect(changes[0].path).toEqual(["list", "0"]);
  });

  it("reports the index for a push, and not the length", () => {
    // Measured: push writes the index and then writes length — but by then
    // length is already 1, and an unchanged write notifies nothing.
    const { state, changes } = track({ list: [] });
    state.list.push("a");
    expect(changes.map((change) => change.path.join("."))).toEqual(["list.0"]);
    expect(state.list).toHaveLength(1);
  });

  it("reports the length when it really changes", () => {
    const { state, changes } = track({ list: [1, 2, 3] });
    state.list.length = 1;
    expect(changes.map((change) => change.path.join("."))).toEqual(["list.length"]);
    expect(state.list).toEqual([1]);
  });

  it("keeps array methods working", () => {
    const { state } = track({ list: [3, 1, 2] });
    expect(state.list.map((n) => n * 2)).toEqual([6, 2, 4]);
    expect(state.list.filter((n) => n > 1)).toEqual([3, 2]);
  });
});

describe("ex092 reactive — identity", () => {
  it("returns the same proxy for the same nested object", () => {
    // Without a cache this is false, and nothing downstream can compare
    // references to decide whether something changed.
    const { state } = track({ user: { name: "ada" } });
    expect(state.user).toBe(state.user);
  });

  it("returns the same proxy through two different paths", () => {
    const shared = { id: 1 };
    const { state } = track({ left: shared, right: shared });
    expect(state.left).toBe(state.right);
  });

  it("leaves primitives unwrapped", () => {
    const { state } = track({ n: 1, s: "a", nil: null });
    expect(state.n).toBe(1);
    expect(state.s).toBe("a");
    expect(state.nil).toBeNull();
  });
});

describe("ex092 toRaw", () => {
  it("unwraps a proxy", () => {
    const target = { user: { name: "ada" } };
    const state = reactive(target, () => {});
    expect(toRaw(state)).toBe(target);
    expect(toRaw(state.user)).toBe(target.user);
  });

  it("passes anything else through", () => {
    const plain = { a: 1 };
    expect(toRaw(plain)).toBe(plain);
    expect(toRaw(5)).toBe(5);
    expect(toRaw(null)).toBeNull();
  });

  it("stores raw values rather than proxies", () => {
    const target = {};
    const other = { nested: { deep: 1 } };
    const state = reactive(target, () => {});
    const otherState = reactive(other, () => {});
    state.borrowed = otherState.nested;
    expect(toRaw(target.borrowed)).toBe(other.nested);
    expect(target.borrowed).toBe(other.nested);
  });
});
