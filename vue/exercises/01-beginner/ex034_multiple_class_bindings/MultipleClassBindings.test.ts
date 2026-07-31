import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import MultipleClassBindings from "./MultipleClassBindings.vue";

describe("MultipleClassBindings", () => {
  it("starts with the static and default size classes only", () => {
    const wrapper = mount(MultipleClassBindings);
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes().sort()).toEqual(["box", "size-small"].sort());
  });

  it("adds the active class after toggling active", async () => {
    const wrapper = mount(MultipleClassBindings);
    await wrapper.get('[data-testid="toggle-active"]').trigger("click");
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes().sort()).toEqual(["active", "box", "size-small"].sort());
  });

  it("switches the size class after toggling size", async () => {
    const wrapper = mount(MultipleClassBindings);
    await wrapper.get('[data-testid="toggle-size"]').trigger("click");
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes().sort()).toEqual(["box", "size-large"].sort());
  });

  it("combines active and large size when both are toggled", async () => {
    const wrapper = mount(MultipleClassBindings);
    await wrapper.get('[data-testid="toggle-active"]').trigger("click");
    await wrapper.get('[data-testid="toggle-size"]').trigger("click");
    const box = wrapper.get('[data-testid="box"]');
    expect(box.classes().sort()).toEqual(["active", "box", "size-large"].sort());
  });
});
