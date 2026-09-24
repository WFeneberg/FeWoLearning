import { describe, expect, it, vi } from "vitest";
import { runFirst, runGroup } from "@ex/04-expert/ex094_structured_concurrency/index.js";

/** A task that resolves when released, or rejects when its signal aborts. */
function controllable(state, name) {
  return (signal) =>
    new Promise((resolve, reject) => {
      state.release[name] = resolve;
      state.running.add(name);
      const finish = (fn) => (value) => {
        state.running.delete(name);
        fn(value);
      };
      state.release[name] = finish(resolve);
      signal.addEventListener(
        "abort",
        () => {
          state.aborted.push(name);
          finish(reject)(signal.reason);
        },
        { once: true },
      );
    });
}

const newState = () => ({ release: {}, running: new Set(), aborted: [] });

describe("ex094 runGroup", () => {
  it("returns results in input order", async () => {
    await expect(
      runGroup([async () => "a", async () => "b"]),
    ).resolves.toEqual(["a", "b"]);
  });

  it("runs the tasks concurrently", async () => {
    let peak = 0;
    let inFlight = 0;
    const task = async () => {
      peak = Math.max(peak, ++inFlight);
      await Promise.resolve();
      inFlight -= 1;
    };
    await runGroup([task, task, task]);
    expect(peak).toBe(3);
  });

  it("gives every task a signal", async () => {
    const seen = [];
    await runGroup([async (signal) => seen.push(signal), async (signal) => seen.push(signal)]);
    expect(seen).toHaveLength(2);
    expect(seen[0]).toBeInstanceOf(AbortSignal);
    expect(seen[0]).toBe(seen[1]);
  });

  it("aborts the siblings when one task fails", async () => {
    const state = newState();
    const failing = async () => {
      throw new Error("first failure");
    };
    await expect(runGroup([controllable(state, "sibling"), failing])).rejects.toThrow(
      "first failure",
    );
    expect(state.aborted).toEqual(["sibling"]);
  });

  it("does not resolve while a sibling is still unwinding", async () => {
    // The difference from Promise.all, which rejects immediately and
    // leaves the sibling running unsupervised.
    const state = newState();
    await expect(
      runGroup([
        controllable(state, "sibling"),
        async () => {
          throw new Error("boom");
        },
      ]),
    ).rejects.toThrow("boom");
    expect(state.running.size).toBe(0);
  });

  it("reports the original failure, not a sibling's abort error", async () => {
    const state = newState();
    await expect(
      runGroup([
        controllable(state, "a"),
        controllable(state, "b"),
        async () => {
          throw new RangeError("the real problem");
        },
      ]),
    ).rejects.toThrow(RangeError);
  });

  it("propagates an outer signal", async () => {
    const state = newState();
    const outer = new AbortController();
    const group = runGroup([controllable(state, "task")], { signal: outer.signal });
    await Promise.resolve();
    outer.abort(new Error("caller cancelled"));
    await expect(group).rejects.toThrow("caller cancelled");
    expect(state.aborted).toEqual(["task"]);
  });

  it("handles an empty task list", async () => {
    await expect(runGroup([])).resolves.toEqual([]);
  });
});

describe("ex094 runFirst", () => {
  it("resolves with the first success and cancels the rest", async () => {
    const state = newState();
    await expect(
      runFirst([controllable(state, "slow"), async () => "quick"]),
    ).resolves.toBe("quick");
    expect(state.aborted).toEqual(["slow"]);
    expect(state.running.size).toBe(0);
  });

  it("ignores a failure while another task can still win", async () => {
    await expect(
      runFirst([
        async () => {
          throw new Error("nope");
        },
        async () => "winner",
      ]),
    ).resolves.toBe("winner");
  });

  it("rejects with an AggregateError when everything fails", async () => {
    const group = runFirst([
      async () => {
        throw new Error("a");
      },
      async () => {
        throw new Error("b");
      },
    ]);
    await expect(group).rejects.toBeInstanceOf(AggregateError);
  });

  it("keeps only the first winner", async () => {
    const second = vi.fn(async () => "second");
    const result = await runFirst([async () => "first", second]);
    expect(result).toBe("first");
  });
});
