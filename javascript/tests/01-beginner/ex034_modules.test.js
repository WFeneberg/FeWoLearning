import { describe, expect, it } from "vitest";
import {
  bumpTwice,
  namespaceKeys,
  readCount,
  sumThenScale,
} from "@ex/01-beginner/ex034_modules/index.js";
import { count, increment } from "@ex/01-beginner/ex034_modules/deps.js";

// deps.js is evaluated once for the whole run, so every fact here works on
// a delta rather than an absolute count.

describe("ex034 live bindings", () => {
  it("reads the current value, not an import-time snapshot", () => {
    const before = readCount();
    increment();
    expect(readCount()).toBe(before + 1);
  });

  it("moves the binding this test file imported too", () => {
    // `count` here and `count` inside index.js are two views of ONE
    // variable in deps.js. A copy would not move.
    const before = count;
    increment();
    expect(count).toBe(before + 1);
    // Anchored to the exercise: deps.js is provided, so the two lines above
    // are true before any work is done.
    expect(readCount()).toBe(count);
  });

  it("bumps twice per call", () => {
    const before = readCount();
    expect(bumpTwice()).toBe(before + 2);
    expect(readCount()).toBe(before + 2);
  });

  it("shares one module instance no matter who imports it", async () => {
    const before = readCount();
    const reimported = await import("@ex/01-beginner/ex034_modules/deps.js");
    reimported.increment();
    expect(readCount()).toBe(before + 1);
  });
});

describe("ex034 default and named exports", () => {
  it("uses both of deps' function exports", () => {
    expect(sumThenScale(2, 3, 4)).toBe(20);
    expect(sumThenScale(0, 0, 9)).toBe(0);
  });
});

describe("ex034 namespace import", () => {
  it("lists every export, with the default under 'default'", () => {
    expect(namespaceKeys()).toEqual(["add", "count", "default", "increment"]);
  });
});

describe("ex034 re-export", () => {
  it("exposes deps' add from this module as well", async () => {
    // Read through a dynamic import rather than a static one: a static
    // `import { add }` of a module that does not export it is an error at
    // load time, which would take this whole FILE down and report 0 tests
    // instead of one failure.
    const module = await import("@ex/01-beginner/ex034_modules/index.js");
    expect(typeof module.add).toBe("function");
    expect(module.add(2, 3)).toBe(5);
  });
});
