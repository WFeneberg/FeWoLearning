import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import DynamicComponentKeepState from "./DynamicComponentKeepState.vue";

describe("DynamicComponentKeepState", () => {
  it("renders panel A's counter starting at 0", () => {
    const wrapper = mount(DynamicComponentKeepState);
    expect(wrapper.find("button").text()).toBe("0");
  });

  it("increments the active panel's local counter on click", async () => {
    const wrapper = mount(DynamicComponentKeepState);
    await wrapper.find("button").trigger("click");
    await wrapper.find("button").trigger("click");
    expect(wrapper.find("button").text()).toBe("2");
  });

  it("resets a panel's counter to its initial value after switching away and back", async () => {
    const wrapper = mount(DynamicComponentKeepState);

    await wrapper.find("button").trigger("click");
    await wrapper.find("button").trigger("click");
    await wrapper.find("button").trigger("click");
    expect(wrapper.find("button").text()).toBe("3");

    (wrapper.vm as unknown as { switchTo: (name: "a" | "b") => void }).switchTo("b");
    await wrapper.vm.$nextTick();
    expect(wrapper.find("button").text()).toBe("100");

    (wrapper.vm as unknown as { switchTo: (name: "a" | "b") => void }).switchTo("a");
    await wrapper.vm.$nextTick();

    // No KeepAlive is used, so panel A is a brand-new component instance:
    // its local counter must be back at its initial value, not the "3" left
    // over from before the switch.
    expect(wrapper.find("button").text()).toBe("0");
  });

  it("does not preserve panel B's incremented count across a remount", async () => {
    const wrapper = mount(DynamicComponentKeepState);

    (wrapper.vm as unknown as { switchTo: (name: "a" | "b") => void }).switchTo("b");
    await wrapper.vm.$nextTick();
    await wrapper.find("button").trigger("click");
    await wrapper.find("button").trigger("click");
    expect(wrapper.find("button").text()).toBe("102");

    (wrapper.vm as unknown as { switchTo: (name: "a" | "b") => void }).switchTo("a");
    await wrapper.vm.$nextTick();
    (wrapper.vm as unknown as { switchTo: (name: "a" | "b") => void }).switchTo("b");
    await wrapper.vm.$nextTick();

    expect(wrapper.find("button").text()).toBe("100");
  });
});
