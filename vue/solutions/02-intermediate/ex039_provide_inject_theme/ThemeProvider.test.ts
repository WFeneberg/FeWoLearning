import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ThemeProvider from "./ThemeProvider.vue";
import ThemedLabel from "./ThemedLabel.vue";

describe("ThemeProvider / ThemedLabel (provide/inject)", () => {
  it("renders the theme provided by an ancestor, without prop drilling", () => {
    const wrapper = mount(ThemeProvider, {
      props: { theme: "dark" },
      slots: { default: ThemedLabel },
    });

    const label = wrapper.find(".themed-label");
    expect(label.exists()).toBe(true);
    expect(label.text()).toBe("Theme: dark");
    expect(label.classes()).toContain("theme-dark");
  });

  it("reflects a different theme value passed to the provider", () => {
    const wrapper = mount(ThemeProvider, {
      props: { theme: "light" },
      slots: { default: ThemedLabel },
    });

    const label = wrapper.find(".themed-label");
    expect(label.text()).toBe("Theme: light");
    expect(label.classes()).toContain("theme-light");
  });

  it("falls back to a default theme when ThemedLabel has no provider ancestor", () => {
    const wrapper = mount(ThemedLabel);

    expect(wrapper.text()).toBe("Theme: none");
    expect(wrapper.classes()).toContain("theme-none");
  });
});
