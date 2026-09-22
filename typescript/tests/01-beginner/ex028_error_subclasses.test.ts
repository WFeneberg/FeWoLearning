import { describe, expect, it } from "vitest";
import { ValidationError, causeChain } from "@ex/01-beginner/ex028_error_subclasses/index";

describe("ex028 ValidationError", () => {
  it("carries its message", () => {
    expect(new ValidationError("bad email", "email").message).toBe("bad email");
  });

  it("names itself", () => {
    expect(new ValidationError("bad email", "email").name).toBe("ValidationError");
  });

  it("records the field", () => {
    expect(new ValidationError("bad email", "email").field).toBe("email");
  });

  it("is both an Error and a ValidationError", () => {
    const error = new ValidationError("bad email", "email");
    expect(error).toBeInstanceOf(Error);
    expect(error).toBeInstanceOf(ValidationError);
  });

  it("keeps the cause when given one", () => {
    const root = new Error("regex failed");
    expect(new ValidationError("bad email", "email", { cause: root }).cause).toBe(root);
  });

  it("has no cause when given none", () => {
    expect(new ValidationError("bad email", "email").cause).toBeUndefined();
  });
});

describe("ex028 causeChain", () => {
  it("returns a single message for an unwrapped error", () => {
    expect(causeChain(new Error("boom"))).toEqual(["boom"]);
  });

  it("walks the chain outermost first", () => {
    const root = new Error("socket closed");
    const middle = new Error("request failed", { cause: root });
    const outer = new ValidationError("could not save", "email", { cause: middle });
    expect(causeChain(outer)).toEqual(["could not save", "request failed", "socket closed"]);
  });

  it("stops at a cause that is not an Error", () => {
    const outer = new Error("wrapped", { cause: "just a string" });
    expect(causeChain(outer)).toEqual(["wrapped"]);
  });
});
