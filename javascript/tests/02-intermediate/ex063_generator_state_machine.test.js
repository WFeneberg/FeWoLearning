import { describe, expect, it } from "vitest";
import {
  countTransitions,
  drive,
  turnstile,
} from "@ex/02-intermediate/ex063_generator_state_machine/index.js";

describe("ex063 turnstile", () => {
  it("starts locked", () => {
    expect(turnstile().next().value).toBe("locked");
  });

  it("opens on a coin and closes on a push", () => {
    const machine = turnstile();
    machine.next();
    expect(machine.next("coin").value).toBe("open");
    expect(machine.next("push").value).toBe("locked");
  });

  it("refuses an impossible transition without losing its state", () => {
    const machine = turnstile();
    machine.next();
    expect(machine.next("push").value).toBe("locked");
    expect(machine.next("nonsense").value).toBe("locked");
    expect(machine.next("coin").value).toBe("open");
    expect(machine.next("coin").value).toBe("open");
  });

  it("never finishes", () => {
    const machine = turnstile();
    machine.next();
    for (let i = 0; i < 20; i++) expect(machine.next("coin").done).toBe(false);
  });

  it("gives each machine its own state", () => {
    const first = turnstile();
    const second = turnstile();
    first.next();
    second.next();
    first.next("coin");
    expect(second.next("push").value).toBe("locked");
  });
});

describe("ex063 drive", () => {
  it("reports the initial state plus one per event", () => {
    expect(drive(["coin", "push"])).toEqual(["locked", "open", "locked"]);
  });

  it("reports the unchanged state for a refused event", () => {
    expect(drive(["push", "coin", "coin"])).toEqual(["locked", "locked", "open", "open"]);
  });

  it("reports just the initial state for no events", () => {
    expect(drive([])).toEqual(["locked"]);
  });

  it("starts a fresh machine each time", () => {
    drive(["coin"]);
    expect(drive([])).toEqual(["locked"]);
  });
});

describe("ex063 countTransitions", () => {
  it("counts only the events that moved the machine", () => {
    expect(countTransitions(["coin", "push"])).toBe(2);
    expect(countTransitions(["push", "coin", "coin", "push"])).toBe(2);
    expect(countTransitions([])).toBe(0);
    expect(countTransitions(["nonsense"])).toBe(0);
  });
});
