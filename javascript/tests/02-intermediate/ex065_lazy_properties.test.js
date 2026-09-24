import { describe, expect, it, vi } from "vitest";
import { defineLazy, makeConfig } from "@ex/02-intermediate/ex065_lazy_properties/index.js";

describe("ex065 defineLazy", () => {
  it("does not compute until the property is read", () => {
    const compute = vi.fn(() => "value");
    const object = defineLazy({}, "lazy", compute);
    expect(compute).not.toHaveBeenCalled();
    expect(object.lazy).toBe("value");
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it("computes exactly once across many reads", () => {
    const compute = vi.fn(() => ({ built: true }));
    const object = defineLazy({}, "lazy", compute);
    const first = object.lazy;
    const second = object.lazy;
    expect(first).toBe(second);
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it("computes once even when the value is undefined", () => {
    const compute = vi.fn(() => undefined);
    const object = defineLazy({}, "lazy", compute);
    void object.lazy;
    void object.lazy;
    expect(compute).toHaveBeenCalledTimes(1);
  });

  it("replaces the accessor with a plain data property", () => {
    const object = defineLazy({}, "lazy", () => 1);
    expect(Object.getOwnPropertyDescriptor(object, "lazy").get).toBeTypeOf("function");
    void object.lazy;
    const after = Object.getOwnPropertyDescriptor(object, "lazy");
    expect(after.get).toBeUndefined();
    expect(after).toEqual({
      value: 1,
      writable: true,
      enumerable: true,
      configurable: true,
    });
  });

  it("is enumerable before and after the first read", () => {
    const object = defineLazy({}, "lazy", () => 1);
    expect(Object.keys(object)).toEqual(["lazy"]);
    void object.lazy;
    expect(Object.keys(object)).toEqual(["lazy"]);
  });

  it("returns the object it was given", () => {
    const object = {};
    expect(defineLazy(object, "lazy", () => 1)).toBe(object);
  });

  it("can be overwritten after materialising", () => {
    const object = defineLazy({}, "lazy", () => 1);
    void object.lazy;
    object.lazy = 2;
    expect(object.lazy).toBe(2);
  });

  it("keeps two lazy properties independent", () => {
    const first = vi.fn(() => "a");
    const second = vi.fn(() => "b");
    const object = defineLazy(defineLazy({}, "a", first), "b", second);
    expect(object.a).toBe("a");
    expect(second).not.toHaveBeenCalled();
  });
});

describe("ex065 makeConfig", () => {
  it("reports itself unloaded until the settings are read", () => {
    const load = vi.fn(() => ({ port: 8080 }));
    const config = makeConfig(load);
    expect(config.loadedYet).toBe(false);
    expect(load).not.toHaveBeenCalled();
    expect(config.settings).toEqual({ port: 8080 });
    expect(config.loadedYet).toBe(true);
  });

  it("loads once", () => {
    const load = vi.fn(() => ({}));
    const config = makeConfig(load);
    void config.settings;
    void config.settings;
    expect(load).toHaveBeenCalledTimes(1);
  });
});
