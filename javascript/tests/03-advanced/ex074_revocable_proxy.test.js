import { describe, expect, it, vi } from "vitest";
import {
  afterRevocation,
  share,
  withTemporaryAccess,
} from "@ex/03-advanced/ex074_revocable_proxy/index.js";

describe("ex074 share", () => {
  it("passes reads and writes through before revocation", () => {
    const target = { value: 1 };
    const { view } = share(target);
    expect(view.value).toBe(1);
    view.value = 2;
    expect(target.value).toBe(2);
  });

  it("cuts every operation off after revocation", () => {
    const { view, revoke } = share({ value: 1 });
    revoke();
    expect(() => view.value).toThrow(TypeError);
    expect(() => Object.keys(view)).toThrow(TypeError);
    expect(() => "value" in view).toThrow(TypeError);
  });

  it("leaves the target usable by whoever still holds it", () => {
    const target = { value: 1 };
    const { revoke } = share(target);
    revoke();
    expect(target.value).toBe(1);
    target.value = 3;
    expect(target.value).toBe(3);
  });

  it("can be revoked twice without complaint", () => {
    const { revoke } = share({});
    revoke();
    expect(() => revoke()).not.toThrow();
  });
});

describe("ex074 withTemporaryAccess", () => {
  it("returns the callback's result", () => {
    expect(withTemporaryAccess({ value: 7 }, (view) => view.value)).toBe(7);
  });

  it("revokes the view afterwards", () => {
    let leaked;
    withTemporaryAccess({ value: 7 }, (view) => {
      leaked = view;
    });
    expect(() => leaked.value).toThrow(TypeError);
  });

  it("revokes even when the callback throws", () => {
    let leaked;
    expect(() =>
      withTemporaryAccess({ value: 7 }, (view) => {
        leaked = view;
        throw new Error("boom");
      }),
    ).toThrow("boom");
    expect(() => leaked.value).toThrow(TypeError);
  });

  it("gives each call its own view", () => {
    const resource = { value: 1 };
    const fn = vi.fn((view) => view.value);
    expect(withTemporaryAccess(resource, fn)).toBe(1);
    expect(withTemporaryAccess(resource, fn)).toBe(1);
    // Object.is, not expect(...).toBe: measured, Vitest's matchers perform
    // a 'has' on the value, which a revoked proxy refuses with a TypeError.
    expect(Object.is(fn.mock.calls[0][0], fn.mock.calls[1][0])).toBe(false);
  });
});

describe("ex074 afterRevocation", () => {
  it("keeps typeof and identity, and nothing else", () => {
    expect(afterRevocation()).toEqual({
      read: "TypeError",
      write: "TypeError",
      typeOf: "object",
      isProxyEqual: true,
    });
  });
});
