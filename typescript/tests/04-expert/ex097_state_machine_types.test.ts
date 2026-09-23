import { describe, expect, it } from "vitest";
import { machine } from "@ex/04-expert/ex097_state_machine_types/index";

describe("ex097 machine", () => {
  it("starts where it was told to", () => {
    expect(machine("idle").state).toBe("idle");
  });

  it("moves on a legal event", () => {
    expect(machine("idle").send("start").state).toBe("running");
  });

  it("follows a whole path", () => {
    expect(machine("idle").send("start").send("pause").send("resume").send("finish").state).toBe(
      "done",
    );
  });

  it("has two ways out of running", () => {
    expect(machine("running").send("pause").state).toBe("paused");
    expect(machine("running").send("finish").state).toBe("done");
  });

  it("gives a fresh machine rather than mutating", () => {
    const start = machine("idle");
    start.send("start");
    expect(start.state).toBe("idle");
  });
});
