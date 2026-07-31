import { afterEach, describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import ClickOutside from "./ClickOutside.vue";

afterEach(() => {
  document.body.innerHTML = "";
});

describe("ClickOutside", () => {
  it("does not emit outside when clicking inside the bound element", async () => {
    const wrapper = mount(ClickOutside, { attachTo: document.body });
    await wrapper.get('[data-testid="box"]').trigger("click");
    expect(wrapper.emitted("outside")).toBeUndefined();
    wrapper.unmount();
  });

  it("emits outside when clicking outside the bound element", async () => {
    const wrapper = mount(ClickOutside, { attachTo: document.body });
    await wrapper.get('[data-testid="outside"]').trigger("click");
    expect(wrapper.emitted("outside")).toHaveLength(1);
    wrapper.unmount();
  });

  it("adds a document click listener on mount and removes it on unmount", () => {
    const addSpy = vi.spyOn(document, "addEventListener");
    const removeSpy = vi.spyOn(document, "removeEventListener");

    const wrapper = mount(ClickOutside, { attachTo: document.body });
    const clickCall = addSpy.mock.calls.find(([type]) => type === "click");
    expect(clickCall).toBeDefined();
    const [, listener] = clickCall!;

    wrapper.unmount();
    expect(removeSpy).toHaveBeenCalledWith("click", listener);

    addSpy.mockRestore();
    removeSpy.mockRestore();
  });
});
