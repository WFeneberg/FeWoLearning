import { describe, expect, it } from "vitest";
import {
  callsBeforeDeclaration,
  tdzErrorName,
  typeofUndeclared,
  varSurvivesBlock,
} from "@ex/01-beginner/ex017_scope_and_hoisting/index.js";

describe("ex017 var vs let", () => {
  it("lets a var escape the block it was written in", () => {
    expect(varSurvivesBlock()).toBe("from block");
  });

  it("reports a ReferenceError for a let touched before its declaration", () => {
    // Not "undefined" — that is what a var would give. A let exists but is
    // unreachable until its declaration executes.
    expect(tdzErrorName()).toBe("ReferenceError");
  });
});

describe("ex017 hoisting", () => {
  it("can call a function declaration written further down", () => {
    expect(callsBeforeDeclaration()).toBe("hoisted");
  });

  it("answers 'undefined' for a name that does not exist at all", () => {
    // typeof is the only operator that may touch an undeclared name. Any
    // other use of it throws a ReferenceError.
    expect(typeofUndeclared()).toBe("undefined");
  });
});
