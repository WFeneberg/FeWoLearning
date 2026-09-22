import { describe, expect, it } from "vitest";
import { greet } from "@ex/01-beginner/ex001_structural_typing/index";

describe("ex001 greet", () => {
  it("greets a plain object of the right shape", () => {
    expect(greet({ name: "Ada" })).toBe("Hello, Ada!");
  });

  it("accepts a class instance that never names the interface", () => {
    // Robot does not `implements Named`. It does not import it. It matches
    // the shape, and that is the whole requirement.
    class Robot {
      constructor(public readonly name: string) {}
    }
    expect(greet(new Robot("R2"))).toBe("Hello, R2!");
  });

  it("accepts an object carrying extra properties", () => {
    const user = { name: "Grace", id: 7, admin: true };
    expect(greet(user)).toBe("Hello, Grace!");
  });
});
