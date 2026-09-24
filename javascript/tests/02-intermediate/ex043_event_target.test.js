import { describe, expect, it, vi } from "vitest";
import { addTwiceCallCount, Bus } from "@ex/02-intermediate/ex043_event_target/index.js";

describe("ex043 Bus", () => {
  it("delivers the detail, not the event object", () => {
    const bus = new Bus();
    const fn = vi.fn();
    bus.on("tick", fn);
    bus.emit("tick", { n: 1 });
    expect(fn).toHaveBeenCalledWith({ n: 1 });
  });

  it("delivers synchronously", () => {
    // EventTarget dispatch is not queued — the listener has run by the time
    // emit() returns.
    const bus = new Bus();
    const seen = [];
    bus.on("tick", (detail) => seen.push(detail));
    bus.emit("tick", 1);
    expect(seen).toEqual([1]);
  });

  it("ignores events nobody is listening for", () => {
    expect(() => new Bus().emit("nobody", 1)).not.toThrow();
  });

  it("keeps listeners for different names apart", () => {
    const bus = new Bus();
    const a = vi.fn();
    const b = vi.fn();
    bus.on("a", a);
    bus.on("b", b);
    bus.emit("a", 1);
    expect(a).toHaveBeenCalledTimes(1);
    expect(b).not.toHaveBeenCalled();
  });

  it("calls every listener of the same name, in registration order", () => {
    const bus = new Bus();
    const order = [];
    bus.on("tick", () => order.push("first"));
    bus.on("tick", () => order.push("second"));
    bus.emit("tick", null);
    expect(order).toEqual(["first", "second"]);
  });

  it("unsubscribes through the returned function", () => {
    const bus = new Bus();
    const fn = vi.fn();
    const off = bus.on("tick", fn);
    bus.emit("tick", 1);
    off();
    bus.emit("tick", 2);
    expect(fn).toHaveBeenCalledTimes(1);
  });

  it("is an EventTarget, so the raw API works too", () => {
    const bus = new Bus();
    const fn = vi.fn();
    bus.addEventListener("tick", fn);
    bus.emit("tick", "raw");
    expect(fn.mock.calls[0][0]).toBeInstanceOf(CustomEvent);
    expect(fn.mock.calls[0][0].detail).toBe("raw");
  });
});

describe("ex043 once", () => {
  it("fires exactly once", () => {
    const bus = new Bus();
    const fn = vi.fn();
    bus.once("tick", fn);
    bus.emit("tick", 1);
    bus.emit("tick", 2);
    expect(fn).toHaveBeenCalledTimes(1);
    expect(fn).toHaveBeenCalledWith(1);
  });

  it("can be cancelled before it ever fires", () => {
    const bus = new Bus();
    const fn = vi.fn();
    bus.once("tick", fn)();
    bus.emit("tick", 1);
    expect(fn).not.toHaveBeenCalled();
  });
});

describe("ex043 addTwiceCallCount", () => {
  it("is 1 — a duplicate registration of the same function is dropped", () => {
    expect(addTwiceCallCount()).toBe(1);
  });
});
