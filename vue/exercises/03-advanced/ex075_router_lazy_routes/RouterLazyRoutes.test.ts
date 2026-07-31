import { describe, expect, it } from "vitest";
import { flushPromises, mount } from "@vue/test-utils";
import RouterLazyRoutes from "./RouterLazyRoutes.vue";

describe("RouterLazyRoutes", () => {
  it("renders the initial `/` route once its lazy component resolves", async () => {
    const wrapper = mount(RouterLazyRoutes);
    await flushPromises();
    expect(wrapper.text()).toContain("Home page");
    expect(wrapper.text()).not.toContain("Settings");
  });

  it("navigates to /settings and renders the lazily loaded component", async () => {
    const wrapper = mount(RouterLazyRoutes);
    await flushPromises();

    wrapper.vm.navigate("/settings");
    await flushPromises();

    expect(wrapper.vm.currentPath).toBe("/settings");
    expect(wrapper.text()).toContain("Settings page loaded lazily");
    expect(wrapper.text()).not.toContain("Home page");
  });

  it("does not render the settings route's content before navigation", () => {
    const wrapper = mount(RouterLazyRoutes);
    expect(wrapper.text()).not.toContain("Settings page loaded lazily");
  });
});
