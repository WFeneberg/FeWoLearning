import { describe, expect, it, vi } from "vitest";
import { describeUser, effect, run } from "@ex/04-expert/ex093_effects_runtime/index.js";

const handlers = (overrides = {}) => ({
  readUser: async (id) => ({ name: `user-${id}` }),
  readOrders: async () => ["a", "b", "c"],
  ...overrides,
});

describe("ex093 effect", () => {
  it("is plain data", () => {
    expect(effect("readUser", 7)).toEqual({ type: "readUser", payload: 7 });
  });
});

describe("ex093 run", () => {
  it("feeds each handler's result back into the program", async () => {
    const program = (function* () {
      const a = yield effect("double", 2);
      const b = yield effect("double", a);
      return b;
    })();
    await expect(run(program, { double: (n) => n * 2 })).resolves.toBe(8);
  });

  it("awaits an async handler", async () => {
    const program = (function* () {
      return yield effect("slow", 1);
    })();
    await expect(run(program, { slow: async (n) => n + 1 })).resolves.toBe(2);
  });

  it("throws a handler's failure INTO the generator", async () => {
    const program = (function* () {
      try {
        yield effect("fail", null);
        return "not reached";
      } catch (error) {
        return `caught ${error.message}`;
      }
    })();
    await expect(
      run(program, {
        fail: () => {
          throw new Error("handler exploded");
        },
      }),
    ).resolves.toBe("caught handler exploded");
  });

  it("lets an uncaught failure out of run()", async () => {
    const program = (function* () {
      yield effect("fail", null);
    })();
    await expect(
      run(program, {
        fail: () => {
          throw new Error("no catch anywhere");
        },
      }),
    ).rejects.toThrow("no catch anywhere");
  });

  it("rejects on an unknown effect", async () => {
    const program = (function* () {
      yield effect("nonexistent", null);
    })();
    await expect(run(program, {})).rejects.toThrow(RangeError);
  });

  it("still runs the program's finally on an unknown effect", async () => {
    const cleanup = vi.fn();
    const program = (function* () {
      try {
        yield effect("nonexistent", null);
      } finally {
        cleanup();
      }
    })();
    await expect(run(program, {})).rejects.toThrow(RangeError);
    expect(cleanup).toHaveBeenCalledTimes(1);
  });

  it("returns a program that yields nothing", async () => {
    const program = (function* () {
      return "immediate";
    })();
    await expect(run(program, {})).resolves.toBe("immediate");
  });
});

describe("ex093 describeUser", () => {
  it("runs against test handlers with no mocking at all", async () => {
    await expect(run(describeUser(7), handlers())).resolves.toBe("user-7 has 3 orders");
  });

  it("asks for the effects it needs, with the right payloads", async () => {
    const readUser = vi.fn(async (id) => ({ name: `user-${id}` }));
    const readOrders = vi.fn(async () => []);
    await run(describeUser(42), handlers({ readUser, readOrders }));
    expect(readUser).toHaveBeenCalledWith(42);
    expect(readOrders).toHaveBeenCalledWith(42);
  });

  it("performs no effect of its own — the generator is inert until run", () => {
    const program = describeUser(1);
    const first = program.next();
    expect(first.done).toBe(false);
    expect(first.value).toEqual({ type: "readUser", payload: 1 });
  });

  it("degrades when the orders effect fails", async () => {
    await expect(
      run(
        describeUser(7),
        handlers({
          readOrders: async () => {
            throw new Error("offline");
          },
        }),
      ),
    ).resolves.toBe("user-7 has unknown orders");
  });

  it("does not swallow a failure of the first effect", async () => {
    await expect(
      run(
        describeUser(7),
        handlers({
          readUser: async () => {
            throw new Error("no user service");
          },
        }),
      ),
    ).rejects.toThrow("no user service");
  });
});
