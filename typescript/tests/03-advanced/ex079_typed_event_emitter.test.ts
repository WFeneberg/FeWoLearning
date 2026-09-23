import { describe, expect, it } from "vitest";
import { Emitter } from "@ex/03-advanced/ex079_typed_event_emitter/index";
import type { AppEvents } from "@ex/03-advanced/ex079_typed_event_emitter/index";

describe("ex079 Emitter", () => {
  it("delivers a payload to its handler", () => {
    const emitter = new Emitter<AppEvents>();
    const seen: { x: number; y: number }[] = [];
    emitter.on("click", (payload) => seen.push(payload as { x: number; y: number }));
    emitter.emit("click", { x: 1, y: 2 } as never);
    expect(seen).toEqual([{ x: 1, y: 2 }]);
  });

  it("keeps events apart", () => {
    const emitter = new Emitter<AppEvents>();
    const clicks: unknown[] = [];
    const keys: unknown[] = [];
    emitter.on("click", (p) => clicks.push(p));
    emitter.on("key", (p) => keys.push(p));
    emitter.emit("key", { code: "Enter" } as never);
    expect(clicks).toEqual([]);
    expect(keys).toEqual([{ code: "Enter" }]);
  });

  it("calls every handler for an event, in registration order", () => {
    const emitter = new Emitter<AppEvents>();
    const order: string[] = [];
    emitter.on("key", () => order.push("first"));
    emitter.on("key", () => order.push("second"));
    emitter.emit("key", { code: "a" } as never);
    expect(order).toEqual(["first", "second"]);
  });

  it("emits a payload-free event", () => {
    const emitter = new Emitter<AppEvents>();
    let closed = 0;
    emitter.on("close", () => (closed += 1));
    emitter.emit("close" as never);
    expect(closed).toBe(1);
  });

  it("does nothing for an event nobody listens to", () => {
    const emitter = new Emitter<AppEvents>();
    expect(() => emitter.emit("click", { x: 0, y: 0 } as never)).not.toThrow();
  });

  it("counts its handlers", () => {
    const emitter = new Emitter<AppEvents>();
    expect(emitter.countFor("click")).toBe(0);
    emitter.on("click", () => undefined);
    emitter.on("click", () => undefined);
    expect(emitter.countFor("click")).toBe(2);
    expect(emitter.countFor("key")).toBe(0);
  });
});
