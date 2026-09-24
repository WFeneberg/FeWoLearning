import { describe, expect, it } from "vitest";
import {
  currentContext,
  idAfterAwait,
  withContext,
  withExtra,
} from "@ex/03-advanced/ex087_async_local_storage/index.js";

describe("ex087 withContext", () => {
  it("makes the context readable inside", () => {
    expect(withContext({ id: "r1" }, () => currentContext().id)).toBe("r1");
  });

  it("returns the callback's result", () => {
    expect(withContext({}, () => 42)).toBe(42);
  });

  it("is undefined outside any run", () => {
    expect(currentContext()).toBeUndefined();
  });

  it("restores the outer context afterwards", () => {
    withContext({ id: "outer" }, () => {
      withContext({ id: "inner" }, () => {
        expect(currentContext().id).toBe("inner");
      });
      expect(currentContext().id).toBe("outer");
    });
    expect(currentContext()).toBeUndefined();
  });
});

describe("ex087 across awaits", () => {
  it("survives an await", async () => {
    await expect(withContext({ id: "r1" }, () => idAfterAwait())).resolves.toBe("r1");
  });

  it("keeps two concurrent chains apart", async () => {
    // The fact that makes this usable: the store follows each async chain,
    // so a shared module-level variable would be wrong here and this is not.
    const [first, second] = await Promise.all([
      withContext({ id: "a" }, () => idAfterAwait()),
      withContext({ id: "b" }, () => idAfterAwait()),
    ]);
    expect([first, second]).toEqual(["a", "b"]);
  });

  it("is undefined again after the run resolves", async () => {
    await withContext({ id: "r1" }, () => idAfterAwait());
    expect(currentContext()).toBeUndefined();
  });

  it("follows a setTimeout scheduled inside the run", async () => {
    const seen = await withContext(
      { id: "timer" },
      () => new Promise((resolve) => setTimeout(() => resolve(currentContext()?.id), 0)),
    );
    expect(seen).toBe("timer");
  });
});

describe("ex087 withExtra", () => {
  it("extends the current context", () => {
    withContext({ id: "r1" }, () => {
      withExtra({ user: "ada" }, () => {
        expect(currentContext()).toEqual({ id: "r1", user: "ada" });
      });
    });
  });

  it("leaves the outer context alone", () => {
    withContext({ id: "r1" }, () => {
      withExtra({ user: "ada" }, () => {});
      expect(currentContext()).toEqual({ id: "r1" });
    });
  });

  it("works with no outer context at all", () => {
    expect(withExtra({ user: "ada" }, () => currentContext())).toEqual({ user: "ada" });
  });
});
