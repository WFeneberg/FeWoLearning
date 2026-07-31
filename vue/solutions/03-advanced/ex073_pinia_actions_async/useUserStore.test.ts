import { describe, expect, it } from "vitest";
import { useUserStore, type User } from "./useUserStore";

// A "deferred" lets the test control exactly when the mock fetch resolves,
// so loading-state transitions can be asserted deterministically without
// real timers or wall-clock delays.
function createDeferred<T>() {
  let resolve!: (value: T) => void;
  let reject!: (reason: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

const mockUser: User = { id: 7, name: "Ada Lovelace", email: "ada@example.com" };

describe("useUserStore", () => {
  it("starts with no user, not loading, and no error", () => {
    const store = useUserStore();
    expect(store.user.value).toBeNull();
    expect(store.loading.value).toBe(false);
    expect(store.error.value).toBeNull();
  });

  it("sets user to the resolved data after loadUser awaits the fetcher", async () => {
    const store = useUserStore();
    const fetchUser = async () => mockUser;

    await store.loadUser(fetchUser);

    expect(store.user.value).toEqual(mockUser);
    expect(store.loading.value).toBe(false);
    expect(store.error.value).toBeNull();
  });

  it("is loading while the fetcher's promise is pending, then not once it resolves", async () => {
    const store = useUserStore();
    const deferred = createDeferred<User>();

    const call = store.loadUser(() => deferred.promise);
    expect(store.loading.value).toBe(true);
    expect(store.user.value).toBeNull();

    deferred.resolve(mockUser);
    await call;

    expect(store.loading.value).toBe(false);
    expect(store.user.value).toEqual(mockUser);
  });

  it("records an error and leaves user unset when the fetcher rejects", async () => {
    const store = useUserStore();
    const fetchUser = async (): Promise<User> => {
      throw new Error("network down");
    };

    await store.loadUser(fetchUser);

    expect(store.user.value).toBeNull();
    expect(store.loading.value).toBe(false);
    expect(store.error.value).toBe("network down");
  });

  it("clears a previous error on a subsequent successful load", async () => {
    const store = useUserStore();
    await store.loadUser(async () => {
      throw new Error("first failure");
    });
    expect(store.error.value).toBe("first failure");

    await store.loadUser(async () => mockUser);

    expect(store.error.value).toBeNull();
    expect(store.user.value).toEqual(mockUser);
  });
});
