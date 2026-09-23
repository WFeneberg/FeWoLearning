import { expectTypeOf, test } from "vitest";
import { Emitter } from "@ex/03-advanced/ex079_typed_event_emitter/index";
import type { AppEvents } from "@ex/03-advanced/ex079_typed_event_emitter/index";

const emitter = new Emitter<AppEvents>();

// The handler's parameter is contextually typed from the event NAME, so
// no annotation is written at any call site below.
test("a handler receives that event's own payload", () => {
  emitter.on("click", (payload) => {
    expectTypeOf(payload).toEqualTypeOf<{ x: number; y: number }>();
  });
  emitter.on("key", (payload) => {
    expectTypeOf(payload).toEqualTypeOf<{ code: string }>();
  });
});

test("emit demands the matching payload", () => {
  emitter.emit("click", { x: 1, y: 2 });
  // @ts-expect-error — that is a key payload, not a click payload
  emitter.emit("click", { code: "Enter" });
});

// The variadic tuple: a void event takes no second argument, and
// supplying one is an error.
test("a void event is emitted with no payload at all", () => {
  emitter.emit("close");
  // @ts-expect-error — close carries nothing
  emitter.emit("close", { x: 1 });
});

test("a payload-carrying event may not omit its payload", () => {
  // @ts-expect-error — click needs its payload
  emitter.emit("click");
});

test("an unknown event name is rejected", () => {
  // @ts-expect-error — "scroll" is not in AppEvents
  emitter.on("scroll", () => undefined);
});
