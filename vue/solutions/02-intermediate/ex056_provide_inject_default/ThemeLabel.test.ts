import { describe, expect, it } from "vitest";
import { defineComponent, h } from "vue";
import { mount } from "@vue/test-utils";
import ThemeLabel from "./ThemeLabel.vue";
import { themeKey } from "./themeKey";

describe("ThemeLabel", () => {
  it("falls back to the default when no ancestor provides a theme", () => {
    const wrapper = mount(ThemeLabel);
    expect(wrapper.text()).toBe("light");
  });

  it("uses the ancestor's provided value when present", () => {
    const Parent = defineComponent({
      setup() {
        return () => h(ThemeLabel);
      },
    });

    const wrapper = mount(Parent, {
      global: {
        provide: {
          [themeKey as symbol]: "dark",
        },
      },
    });

    expect(wrapper.text()).toBe("dark");
  });

  it("still falls back when the ancestor provides an unrelated key", () => {
    const otherKey = Symbol("other");
    const Parent = defineComponent({
      setup() {
        return () => h(ThemeLabel);
      },
    });

    const wrapper = mount(Parent, {
      global: {
        provide: {
          [otherKey as symbol]: "irrelevant",
        },
      },
    });

    expect(wrapper.text()).toBe("light");
  });
});
