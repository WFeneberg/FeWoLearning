// Exercise 079 — an emitter that knows each event's payload (advanced).
// Goal:   let one object carry many differently shaped events without a
//         cast anywhere.
// Drills: an event map as a type parameter, per-event payload inference,
//         a variadic tuple for the no-payload case.
// Passes: each handler receives that event's own payload, a wrong payload
//         is rejected, and a payload-free event is emitted with no second
//         argument at all.
//
// The map is the design: an interface whose keys are event names and
// whose values are payloads. `on<K extends keyof E>(name: K, handler:
// (payload: E[K]) => void)` then infers K from the name LITERAL, which
// makes the handler's parameter that event's payload and nothing else.
//
// The awkward case is an event with no payload, and it is worth doing
// properly. `emit("close", undefined)` is ugly; `emit("close")` needs the
// second parameter to disappear. A VARIADIC TUPLE in the parameter list
// does it:
//
//   emit<K>(name: K, ...args: E[K] extends void ? [] : [payload: E[K]])
//
// The rest parameter's type is a conditional over the payload, so for a
// void event it is the empty tuple — no second argument, and supplying
// one is an error — and for every other event it is a one-element tuple
// with the payload's own type and label. This is the shape most typed
// emitters in the wild get wrong.

export interface AppEvents {
  click: { x: number; y: number };
  key: { code: string };
  close: void;
}

export class Emitter<E> {
  private readonly handlers = new Map<keyof E, ((payload: never) => void)[]>();

  /** TODO: register a handler for one event, typed by that event. The
   *  stub takes a function type rather than `unknown` so the tests'
   *  unannotated callbacks stay contextually typed instead of becoming
   *  implicit anys. */
  on(_name: string, _handler: (payload: never) => void): void {
    throw new Error("TODO: implement on");
  }

  /** TODO: call every handler for one event. A void event takes no
   *  payload argument at all. */
  emit(_name: string, ..._args: unknown[]): void {
    throw new Error("TODO: implement emit");
  }

  /** TODO: how many handlers are registered for an event. */
  countFor(_name: string): number {
    throw new Error("TODO: implement countFor");
  }
}
