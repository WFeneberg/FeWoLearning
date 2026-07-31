import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import StyleBinding from "./StyleBinding.vue";

describe("StyleBinding", () => {
  it("applies the color prop as inline style", () => {
    const wrapper = mount(StyleBinding, { props: { color: "red", fontSize: 16 } });
    const text = wrapper.get('[data-testid="text"]');
    expect(text.attributes("style")).toContain("color: red");
  });

  it("applies the fontSize prop as inline style in pixels", () => {
    const wrapper = mount(StyleBinding, { props: { color: "blue", fontSize: 24 } });
    const text = wrapper.get('[data-testid="text"]');
    expect(text.attributes("style")).toContain("font-size: 24px");
  });

  it("updates the style when props change", async () => {
    const wrapper = mount(StyleBinding, { props: { color: "green", fontSize: 12 } });
    await wrapper.setProps({ color: "purple", fontSize: 20 });
    const text = wrapper.get('[data-testid="text"]');
    expect(text.attributes("style")).toContain("color: purple");
    expect(text.attributes("style")).toContain("font-size: 20px");
  });
});
