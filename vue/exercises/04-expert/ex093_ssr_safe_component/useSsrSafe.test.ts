import { beforeEach, describe, expect, it, vi } from "vitest";
import { defineComponent, h, nextTick } from "vue";
import { mount } from "@vue/test-utils";
import { resetIdCounter, useClientOnly, useSsrSafeId } from "./useSsrSafe";

beforeEach(() => {
  resetIdCounter();
});

describe("useSsrSafeId", () => {
  it("counts from 1 per prefix", () => {
    expect(useSsrSafeId("field")).toBe("field-1");
    expect(useSsrSafeId("field")).toBe("field-2");
    expect(useSsrSafeId("field")).toBe("field-3");
  });

  it("keeps separate counters per prefix", () => {
    expect(useSsrSafeId("input")).toBe("input-1");
    expect(useSsrSafeId("label")).toBe("label-1");
    expect(useSsrSafeId("input")).toBe("input-2");
    expect(useSsrSafeId("label")).toBe("label-2");
  });

  it("is deterministic: the same call order yields the same ids", () => {
    const first = [useSsrSafeId("a"), useSsrSafeId("b"), useSsrSafeId("a")];
    resetIdCounter();
    const second = [useSsrSafeId("a"), useSsrSafeId("b"), useSsrSafeId("a")];

    // A server render and the client hydration that follows it must agree.
    expect(second).toEqual(first);
    expect(first).toEqual(["a-1", "b-1", "a-2"]);
  });
});

describe("useClientOnly", () => {
  it("exposes the fallback synchronously without calling the getter", () => {
    const getter = vi.fn(() => "from-browser");

    const Comp = defineComponent({
      setup() {
        const value = useClientOnly(getter, "from-server");
        // Read during setup: this is the server-render moment.
        expect(value.value).toBe("from-server");
        expect(getter).not.toHaveBeenCalled();
        return () => h("span", value.value);
      },
    });

    mount(Comp);
    expect(getter).toHaveBeenCalledTimes(1);
  });

  it("updates the ref to the getter's result once mounted", async () => {
    const Comp = defineComponent({
      setup() {
        const width = useClientOnly(() => 1024, 0);
        return () => h("span", String(width.value));
      },
    });

    const wrapper = mount(Comp);
    await nextTick();

    expect(wrapper.text()).toBe("1024");
  });

  it("lets the getter read browser-only globals safely", async () => {
    const Comp = defineComponent({
      setup() {
        const title = useClientOnly(() => document.title, "(no document)");
        return () => h("span", title.value);
      },
    });

    document.title = "hydrated";
    const wrapper = mount(Comp);
    await nextTick();

    expect(wrapper.text()).toBe("hydrated");
  });

  it("keeps the fallback when used outside a component instance", () => {
    const getter = vi.fn(() => "never");
    const value = useClientOnly(getter, "stays");

    expect(value.value).toBe("stays");
    expect(getter).not.toHaveBeenCalled();
  });
});
