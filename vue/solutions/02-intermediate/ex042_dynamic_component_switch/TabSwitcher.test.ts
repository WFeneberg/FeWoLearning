import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import TabSwitcher from "./TabSwitcher.vue";

describe("TabSwitcher", () => {
  it("renders the alpha tab by default", () => {
    const wrapper = mount(TabSwitcher);
    expect(wrapper.text()).toContain("Alpha content");
    expect(wrapper.text()).not.toContain("Beta content");
    expect(wrapper.text()).not.toContain("Gamma content");
  });

  it("switches to the beta tab's content when currentTab changes", async () => {
    const wrapper = mount(TabSwitcher);
    (wrapper.vm as unknown as { currentTab: string }).currentTab = "beta";
    await wrapper.vm.$nextTick();

    expect(wrapper.text()).toContain("Beta content");
    expect(wrapper.text()).not.toContain("Alpha content");
    expect(wrapper.text()).not.toContain("Gamma content");
  });

  it("switches to the gamma tab's content when currentTab changes", async () => {
    const wrapper = mount(TabSwitcher);
    (wrapper.vm as unknown as { currentTab: string }).currentTab = "gamma";
    await wrapper.vm.$nextTick();

    expect(wrapper.text()).toContain("Gamma content");
    expect(wrapper.text()).not.toContain("Alpha content");
    expect(wrapper.text()).not.toContain("Beta content");
  });
});
