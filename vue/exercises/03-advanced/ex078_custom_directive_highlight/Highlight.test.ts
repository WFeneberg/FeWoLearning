import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import Highlight from "./Highlight.vue";

describe("Highlight", () => {
  it("applies the bound color as the background color on mount", () => {
    const wrapper = mount(Highlight, { props: { color: "rgb(255, 0, 0)" } });
    const box = wrapper.get<HTMLElement>('[data-testid="box"]');
    expect(box.element.style.backgroundColor).toBe("rgb(255, 0, 0)");
  });

  it("updates the background color when the binding value changes", async () => {
    const wrapper = mount(Highlight, { props: { color: "rgb(255, 0, 0)" } });
    await wrapper.setProps({ color: "rgb(0, 128, 0)" });
    const box = wrapper.get<HTMLElement>('[data-testid="box"]');
    expect(box.element.style.backgroundColor).toBe("rgb(0, 128, 0)");
  });

  it("leaves other style properties untouched", () => {
    const wrapper = mount(Highlight, { props: { color: "rgb(0, 0, 255)" } });
    const box = wrapper.get<HTMLElement>('[data-testid="box"]');
    expect(box.element.style.color).toBe("");
  });
});
