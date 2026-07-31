import { describe, expect, it } from "vitest";
import { flushPromises, mount } from "@vue/test-utils";
import AsyncComponentLoader from "./AsyncComponentLoader.vue";

describe("AsyncComponentLoader", () => {
  it("shows nothing (or a loading placeholder) before the async component resolves", () => {
    const wrapper = mount(AsyncComponentLoader, { props: { name: "Ada" } });
    expect(wrapper.text()).not.toContain("Hello, Ada!");
  });

  it("renders the resolved component's content once the promise resolves", async () => {
    const wrapper = mount(AsyncComponentLoader, { props: { name: "Ada" } });
    await flushPromises();
    expect(wrapper.text()).toContain("Hello, Ada!");
    expect(wrapper.find(".remote-greeting").exists()).toBe(true);
  });

  it("re-renders with updated props after resolution", async () => {
    const wrapper = mount(AsyncComponentLoader, { props: { name: "Ada" } });
    await flushPromises();
    await wrapper.setProps({ name: "Grace" });
    await flushPromises();
    expect(wrapper.text()).toContain("Hello, Grace!");
    expect(wrapper.text()).not.toContain("Hello, Ada!");
  });
});
