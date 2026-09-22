import { describe, expect, it } from "vitest";
import { settle } from "@ex/02-intermediate/ex070_awaited_recursive/index";

describe("ex070 settle", () => {
  it("passes a plain value through", async () => {
    await expect(settle(42)).resolves.toBe(42);
  });

  it("unwraps a promise", async () => {
    await expect(settle(Promise.resolve("a"))).resolves.toBe("a");
  });

  it("unwraps a nested promise", async () => {
    await expect(settle(Promise.resolve(Promise.resolve(1)))).resolves.toBe(1);
  });

  // await checks for a callable `then`, not for Promise — which is how
  // pre-Promise libraries still interoperate.
  it("unwraps a hand-written thenable", async () => {
    const thenable = {
      then(onfulfilled: (value: string) => void): void {
        onfulfilled("from a thenable");
      },
    };
    await expect(settle(thenable)).resolves.toBe("from a thenable");
  });
});
