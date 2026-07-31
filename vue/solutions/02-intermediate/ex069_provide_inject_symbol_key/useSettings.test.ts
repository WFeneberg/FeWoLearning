import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";
import { provideSettings, useSettings, SETTINGS_KEY, type Settings } from "./useSettings";

const Child = defineComponent({
  name: "Child",
  setup() {
    const settings = useSettings();
    return () => h("div", `${settings.theme}:${settings.locale}`);
  },
});

function makeParent(settings: Settings) {
  return defineComponent({
    name: "Parent",
    setup() {
      provideSettings(settings);
      return () => h(Child);
    },
  });
}

describe("provide/inject with a Symbol key", () => {
  it("delivers the exact provided value to a descendant", () => {
    const wrapper = mount(makeParent({ theme: "dark", locale: "en-US" }));
    expect(wrapper.text()).toBe("dark:en-US");
  });

  it("keeps SETTINGS_KEY as a genuine Symbol", () => {
    expect(typeof SETTINGS_KEY).toBe("symbol");
  });

  it("throws in a descendant with no ancestor provider", () => {
    const Orphan = defineComponent({
      setup() {
        useSettings();
        return () => h("div");
      },
    });
    expect(() => mount(Orphan)).toThrow(/useSettings/);
  });

  it("isolates different provided values across separate trees", () => {
    const a = mount(makeParent({ theme: "light", locale: "de-DE" }));
    const b = mount(makeParent({ theme: "dark", locale: "fr-FR" }));
    expect(a.text()).toBe("light:de-DE");
    expect(b.text()).toBe("dark:fr-FR");
  });
});
