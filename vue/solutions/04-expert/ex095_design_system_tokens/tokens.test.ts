import { describe, expect, it } from "vitest";
import { defineComponent, h, nextTick, ref } from "vue";
import { mount } from "@vue/test-utils";
import { provideTokens, useTokens } from "./tokens";

/** Renders `color.bg` / `color.fg` / a missing token, so a test can read them. */
const Consumer = defineComponent({
  name: "Consumer",
  setup() {
    const { get, all, cssVars } = useTokens();
    return () =>
      h("div", [
        h("span", { class: "bg" }, get("color.bg", "(none)")),
        h("span", { class: "fg" }, get("color.fg", "(none)")),
        h("span", { class: "count" }, String(Object.keys(all.value).length)),
        h("span", { class: "vars" }, JSON.stringify(cssVars.value)),
      ]);
  },
});

function provider(tokens: Record<string, string>, children: unknown) {
  return defineComponent({
    setup() {
      provideTokens(tokens);
      return () => h("div", [children as never]);
    },
  });
}

describe("design-system tokens", () => {
  it("resolves tokens published by a provider above", () => {
    const Root = provider({ "color.bg": "navy", "color.fg": "white" }, h(Consumer));
    const wrapper = mount(Root);

    expect(wrapper.get(".bg").text()).toBe("navy");
    expect(wrapper.get(".fg").text()).toBe("white");
  });

  it("falls back when a token is absent", () => {
    const Root = provider({ "color.bg": "navy" }, h(Consumer));
    const wrapper = mount(Root);

    expect(wrapper.get(".fg").text()).toBe("(none)");
  });

  it("works with no provider at all", () => {
    const wrapper = mount(Consumer);

    expect(wrapper.get(".bg").text()).toBe("(none)");
    expect(wrapper.get(".count").text()).toBe("0");
  });

  it("merges a nested provider over its parent instead of replacing it", () => {
    const Inner = provider({ "color.bg": "crimson" }, h(Consumer));
    const Outer = provider({ "color.bg": "navy", "color.fg": "white" }, h(Inner));
    const wrapper = mount(Outer);

    // Overridden…
    expect(wrapper.get(".bg").text()).toBe("crimson");
    // …but the parent's other tokens are still visible.
    expect(wrapper.get(".fg").text()).toBe("white");
    expect(wrapper.get(".count").text()).toBe("2");
  });

  it("exposes tokens as CSS custom properties with dots turned into dashes", () => {
    const Root = provider({ "color.bg": "navy", "space.sm": "4px" }, h(Consumer));
    const wrapper = mount(Root);

    expect(JSON.parse(wrapper.get(".vars").text())).toEqual({
      "--color-bg": "navy",
      "--space-sm": "4px",
    });
  });

  it("stays reactive when the provided map is swapped", async () => {
    const theme = ref<Record<string, string>>({ "color.bg": "navy" });
    const Root = defineComponent({
      setup() {
        // Provide a getter-backed map so the provider can change themes.
        const merged = provideTokens(theme.value);
        void merged;
        return () => h("div", [h(Consumer)]);
      },
    });

    const wrapper = mount(Root);
    expect(wrapper.get(".bg").text()).toBe("navy");

    // Mutating the same object the provider was given must propagate.
    theme.value["color.bg"] = "forest";
    await nextTick();

    expect(wrapper.get(".bg").text()).toBe("forest");
  });
});
