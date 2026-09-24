import { describe, expect, it, vi } from "vitest";
import {
  buildStack,
  makeResource,
  transferOwnership,
  withResources,
} from "@ex/03-advanced/ex082_disposables/index.js";

describe("ex082 makeResource", () => {
  it("is disposable", () => {
    const log = [];
    const resource = makeResource("db", log);
    expect(typeof resource[Symbol.dispose]).toBe("function");
    expect(resource.disposed).toBe(false);
    resource[Symbol.dispose]();
    expect(resource.disposed).toBe(true);
    expect(log).toEqual(["db"]);
  });
});

describe("ex082 withResources", () => {
  it("returns the body's result", () => {
    expect(withResources([], () => "result")).toBe("result");
  });

  it("disposes in reverse registration order", () => {
    const log = [];
    withResources([makeResource("first", log), makeResource("second", log)], () => {});
    expect(log).toEqual(["second", "first"]);
  });

  it("disposes nothing before the body has run", () => {
    const log = [];
    withResources([makeResource("a", log)], () => {
      expect(log).toEqual([]);
    });
    expect(log).toEqual(["a"]);
  });

  it("disposes everything even when the body throws", () => {
    const log = [];
    expect(() =>
      withResources([makeResource("a", log), makeResource("b", log)], () => {
        throw new Error("body failed");
      }),
    ).toThrow("body failed");
    expect(log).toEqual(["b", "a"]);
  });

  it("gives the body the stack, so it can register more", () => {
    const log = [];
    withResources([makeResource("outer", log)], (stack) => {
      stack.use(makeResource("inner", log));
    });
    expect(log).toEqual(["inner", "outer"]);
  });
});

describe("ex082 buildStack", () => {
  it("does not release anything until disposed", () => {
    const onRelease = vi.fn();
    const log = [];
    const stack = buildStack({ id: 1 }, onRelease, log);
    expect(onRelease).not.toHaveBeenCalled();
    expect(stack.disposed).toBe(false);
  });

  it("adopts a value that is not itself disposable", () => {
    const onRelease = vi.fn();
    const log = [];
    const value = { id: 1 };
    buildStack(value, onRelease, log).dispose();
    expect(onRelease).toHaveBeenCalledWith(value);
  });

  it("runs the deferred callback first — LIFO again", () => {
    const log = [];
    buildStack({}, () => log.push("released"), log).dispose();
    expect(log).toEqual(["deferred", "released"]);
  });

  it("disposes only once", () => {
    const onRelease = vi.fn();
    const stack = buildStack({}, onRelease, []);
    stack.dispose();
    stack.dispose();
    expect(onRelease).toHaveBeenCalledTimes(1);
  });
});

describe("ex082 transferOwnership", () => {
  it("empties the old stack and keeps the disposers alive in the new one", () => {
    const log = [];
    const { moved, oldDisposed } = transferOwnership(buildStack({}, () => log.push("released"), log));
    expect(oldDisposed).toBe(true);
    expect(log).toEqual([]);
    moved.dispose();
    expect(log).toEqual(["deferred", "released"]);
  });

  it("returns a usable DisposableStack", () => {
    const { moved } = transferOwnership(buildStack({}, () => {}, []));
    expect(moved.disposed).toBe(false);
    expect(typeof moved.dispose).toBe("function");
  });
});
