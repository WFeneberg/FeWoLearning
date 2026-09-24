import { describe, expect, it, vi } from "vitest";
import { Emitter } from "@ex/03-advanced/ex079_event_emitter/index.js";

describe("ex079 on / emit", () => {
  it("calls listeners with the arguments", () => {
    const emitter = new Emitter();
    const listener = vi.fn();
    emitter.on("tick", listener);
    expect(emitter.emit("tick", 1, "two")).toBe(1);
    expect(listener).toHaveBeenCalledWith(1, "two");
  });

  it("calls them in registration order", () => {
    const emitter = new Emitter();
    const order = [];
    emitter.on("tick", () => order.push("first")).on("tick", () => order.push("second"));
    emitter.emit("tick");
    expect(order).toEqual(["first", "second"]);
  });

  it("calls the same function twice when it was registered twice", () => {
    // Unlike EventTarget, which drops the duplicate (ex043).
    const emitter = new Emitter();
    const listener = vi.fn();
    emitter.on("tick", listener);
    emitter.on("tick", listener);
    expect(emitter.emit("tick")).toBe(2);
    expect(listener).toHaveBeenCalledTimes(2);
  });

  it("returns 0 for an event nobody listens to", () => {
    expect(new Emitter().emit("nobody")).toBe(0);
  });

  it("keeps events apart", () => {
    const emitter = new Emitter();
    const other = vi.fn();
    emitter.on("a", vi.fn()).on("b", other);
    emitter.emit("a");
    expect(other).not.toHaveBeenCalled();
  });

  it("does not call a listener added during the same emit", () => {
    const emitter = new Emitter();
    const late = vi.fn();
    emitter.on("tick", () => emitter.on("tick", late));
    emitter.emit("tick");
    expect(late).not.toHaveBeenCalled();
    emitter.emit("tick");
    expect(late).toHaveBeenCalledTimes(1);
  });
});

describe("ex079 once", () => {
  it("fires once", () => {
    const emitter = new Emitter();
    const listener = vi.fn();
    emitter.once("tick", listener);
    emitter.emit("tick", "first");
    emitter.emit("tick", "second");
    expect(listener).toHaveBeenCalledTimes(1);
    expect(listener).toHaveBeenCalledWith("first");
  });

  it("stops counting as a listener afterwards", () => {
    const emitter = new Emitter();
    emitter.once("tick", () => {});
    expect(emitter.listenerCount("tick")).toBe(1);
    emitter.emit("tick");
    expect(emitter.listenerCount("tick")).toBe(0);
  });

  it("can be removed before it fires, by the function that was passed in", () => {
    const emitter = new Emitter();
    const listener = vi.fn();
    emitter.once("tick", listener);
    emitter.off("tick", listener);
    emitter.emit("tick");
    expect(listener).not.toHaveBeenCalled();
  });
});

describe("ex079 off", () => {
  it("removes one registration, not all of them", () => {
    const emitter = new Emitter();
    const listener = vi.fn();
    emitter.on("tick", listener).on("tick", listener);
    emitter.off("tick", listener);
    emitter.emit("tick");
    expect(listener).toHaveBeenCalledTimes(1);
  });

  it("is a no-op for an unknown listener or event", () => {
    const emitter = new Emitter();
    expect(() => emitter.off("nothing", () => {})).not.toThrow();
    expect(emitter.off("nothing", () => {})).toBe(emitter);
  });
});

describe("ex079 listener errors", () => {
  it("runs every listener even when one throws", () => {
    const emitter = new Emitter();
    const after = vi.fn();
    emitter.on("tick", () => {
      throw new Error("bad listener");
    });
    emitter.on("tick", after);
    expect(() => emitter.emit("tick")).toThrow(AggregateError);
    expect(after).toHaveBeenCalledTimes(1);
  });

  it("collects every error", () => {
    const emitter = new Emitter();
    emitter.on("tick", () => {
      throw new Error("one");
    });
    emitter.on("tick", () => {
      throw new Error("two");
    });
    try {
      emitter.emit("tick");
      expect.unreachable("should have thrown");
    } catch (error) {
      expect(error.errors.map((e) => e.message)).toEqual(["one", "two"]);
    }
  });

  it("does not throw when nothing failed", () => {
    const emitter = new Emitter();
    emitter.on("tick", () => {});
    expect(() => emitter.emit("tick")).not.toThrow();
  });
});

describe("ex079 listenerCount", () => {
  it("counts per event", () => {
    const emitter = new Emitter();
    expect(emitter.listenerCount("tick")).toBe(0);
    emitter.on("tick", () => {}).on("tick", () => {});
    expect(emitter.listenerCount("tick")).toBe(2);
    expect(emitter.listenerCount("other")).toBe(0);
  });
});
