// Exercise 079 — a Node-style emitter (advanced).
// Goal:   the other event model, and the decisions EventTarget makes for you.
// Drills: a Map of listener lists, once() wrapping, off() by identity,
//         duplicate registration, and what a throwing listener must not do.
// Passes: the same function CAN be registered twice here (unlike ex043's
//         EventTarget), and one listener throwing does not stop the rest.

export class Emitter {
  /**
   * Registers `listener` for `event`. The same function may be registered
   * more than once, and is then called once per registration.
   * Returns this.
   *
   * TODO: implement.
   */
  on(_event, _listener) {
    throw new Error("TODO: implement on");
  }

  /**
   * Registers a listener removed after its first call. Returns this.
   *
   * TODO: implement.
   */
  once(_event, _listener) {
    throw new Error("TODO: implement once");
  }

  /**
   * Removes ONE registration of `listener` for `event` — the first — and
   * returns this. Removing something that is not there is a no-op.
   * Must also remove a once() listener that has not fired yet.
   *
   * TODO: implement.
   */
  off(_event, _listener) {
    throw new Error("TODO: implement off");
  }

  /**
   * Calls every listener for `event`, in registration order, with `args`,
   * and returns how many were called.
   *
   * A listener that throws must not prevent the others from running: let
   * them all run, then throw an AggregateError with the collected errors.
   * A listener registered DURING the emit is not called by that emit.
   *
   * TODO: implement.
   */
  emit(_event, ..._args) {
    throw new Error("TODO: implement emit");
  }

  /** How many listeners `event` has. TODO: implement. */
  listenerCount(_event) {
    throw new Error("TODO: implement listenerCount");
  }
}
