import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ClassBinding from "./ClassBinding.vue";

describe("ClassBinding", () => {
  it("does not have the active class initially", () => {
    const wrapper = mount(ClassBinding);
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes()).not.toContain("active");
  });

  it("adds the active class after clicking the button", async () => {
    const wrapper = mount(ClassBinding);
    await wrapper.get("button").trigger("click");
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes()).toContain("active");
  });

  it("removes the active class after clicking twice", async () => {
    const wrapper = mount(ClassBinding);
    const button = wrapper.get("button");
    await button.trigger("click");
    await button.trigger("click");
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes()).not.toContain("active");
  });
});
