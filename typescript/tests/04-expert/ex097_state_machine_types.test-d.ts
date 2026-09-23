import { expectTypeOf, test } from "vitest";
import { machine } from "@ex/04-expert/ex097_state_machine_types/index";
import type { EventsFor, Next } from "@ex/04-expert/ex097_state_machine_types/index";

test("EventsFor reads the table", () => {
  expectTypeOf<EventsFor<"idle">>().toEqualTypeOf<"start">();
  expectTypeOf<EventsFor<"running">>().toEqualTypeOf<"pause" | "finish">();
  expectTypeOf<EventsFor<"done">>().toEqualTypeOf<string>();
});

test("Next reads where an event lands", () => {
  expectTypeOf<Next<"idle", "start">>().toEqualTypeOf<"running">();
  expectTypeOf<Next<"running", "finish">>().toEqualTypeOf<"done">();
});

test("Next is never for an event the table forbids", () => {
  expectTypeOf<Next<"idle", "pause">>().toEqualTypeOf<never>();
});

// The state travels with the value: each send reports where it landed.
test("the machine tracks its own state through a chain", () => {
  expectTypeOf(machine("idle").state).toEqualTypeOf<"idle">();
  expectTypeOf(machine("idle").send("start").state).toEqualTypeOf<"running">();
  expectTypeOf(machine("idle").send("start").send("pause").state).toEqualTypeOf<"paused">();
  expectTypeOf(
    machine("idle").send("start").send("pause").send("finish").state,
  ).toEqualTypeOf<"done">();
});

test("an event the current state does not allow is rejected", () => {
  // @ts-expect-error — idle has no "pause"
  machine("idle").send("pause");
});

test("and so is one that is legal only later in the path", () => {
  // @ts-expect-error — running has no "resume"; paused does
  machine("idle").send("start").send("resume");
});
