import { describe, expect, it, vi } from "vitest";
import {
  abortReasons,
  onAbort,
  runUntilAborted,
  withAbort,
} from "@ex/02-intermediate/ex042_abort_controller/index.js";

describe("ex042 withAbort", () => {
  it("passes a value through when nothing aborts", async () => {
    const controller = new AbortController();
    await expect(withAbort(Promise.resolve("ok"), controller.signal)).resolves.toBe("ok");
  });

  it("passes a rejection through", async () => {
    const controller = new AbortController();
    await expect(withAbort(Promise.reject(new Error("upstream")), controller.signal)).rejects.toThrow(
      "upstream",
    );
  });

  it("rejects with the signal's reason when it aborts first", async () => {
    const controller = new AbortController();
    const never = new Promise(() => {});
    const guarded = withAbort(never, controller.signal);
    controller.abort(new Error("cancelled by user"));
    await expect(guarded).rejects.toThrow("cancelled by user");
  });

  it("rejects straight away for a signal that was already aborted", async () => {
    // The case an implementation built only on addEventListener misses: the
    // abort event has already been and gone.
    const signal = AbortSignal.abort(new Error("too late"));
    const never = new Promise(() => {});
    await expect(withAbort(never, signal)).rejects.toThrow("too late");
  });

  it("does not abort an already-settled promise", async () => {
    const controller = new AbortController();
    const guarded = withAbort(Promise.resolve("first"), controller.signal);
    await expect(guarded).resolves.toBe("first");
    controller.abort();
    await expect(guarded).resolves.toBe("first");
  });
});

describe("ex042 abortReasons", () => {
  it("defaults to a DOMException named AbortError", () => {
    const reasons = abortReasons("mine");
    expect(reasons.defaultName).toBe("AbortError");
    expect(reasons.defaultIsDomException).toBe(true);
  });

  it("keeps an explicit reason exactly as given", () => {
    const reason = { code: 7 };
    expect(abortReasons(reason).explicit).toBe(reason);
    expect(abortReasons("string reason").explicit).toBe("string reason");
  });
});

describe("ex042 onAbort", () => {
  it("fires once on abort, with the reason", () => {
    const controller = new AbortController();
    const fn = vi.fn();
    onAbort(controller.signal, fn);
    controller.abort("stop");
    controller.abort("again");
    expect(fn).toHaveBeenCalledTimes(1);
    expect(fn).toHaveBeenCalledWith("stop");
  });

  it("fires synchronously for a signal that is already aborted", () => {
    const fn = vi.fn();
    onAbort(AbortSignal.abort("already"), fn);
    expect(fn).toHaveBeenCalledWith("already");
  });

  it("can be unsubscribed before the abort", () => {
    const controller = new AbortController();
    const fn = vi.fn();
    onAbort(controller.signal, fn)();
    controller.abort();
    expect(fn).not.toHaveBeenCalled();
  });

  it("returns a callable even for an already-aborted signal", () => {
    expect(() => onAbort(AbortSignal.abort(), () => {})()).not.toThrow();
  });
});

describe("ex042 runUntilAborted", () => {
  it("stops after the abort and reports the completed steps", async () => {
    const controller = new AbortController();
    let count = 0;
    const step = vi.fn(async () => {
      if (++count === 3) controller.abort();
    });
    await expect(runUntilAborted(controller.signal, step)).resolves.toBe(3);
    expect(step).toHaveBeenCalledTimes(3);
  });

  it("runs nothing at all for a signal that starts aborted", async () => {
    const step = vi.fn();
    await expect(runUntilAborted(AbortSignal.abort(), step)).resolves.toBe(0);
    expect(step).not.toHaveBeenCalled();
  });

  it("lets a real failure out rather than reporting it as an abort", async () => {
    const controller = new AbortController();
    await expect(
      runUntilAborted(controller.signal, async () => {
        throw new RangeError("a genuine bug");
      }),
    ).rejects.toThrow(RangeError);
  });
});
