import { describe, expect, it } from "vitest";
import { flushPromises, mount } from "@vue/test-utils";
import RouterDynamicParams from "./RouterDynamicParams.vue";

describe("RouterDynamicParams", () => {
  it("renders a not-found state for the initial `/` path", () => {
    const wrapper = mount(RouterDynamicParams);
    expect(wrapper.text()).toContain("Not found");
  });

  it("navigates to /users/42 and renders the user profile with the dynamic id", async () => {
    const wrapper = mount(RouterDynamicParams);

    wrapper.vm.navigate("/users/42");
    await flushPromises();

    expect(wrapper.text()).toContain("User #42");
    expect(wrapper.text()).not.toContain("Not found");
    expect(wrapper.vm.route.params).toEqual({ id: "42" });
  });

  it("re-resolves the id when navigating between different user routes", async () => {
    const wrapper = mount(RouterDynamicParams);

    wrapper.vm.navigate("/users/42");
    await flushPromises();
    expect(wrapper.text()).toContain("User #42");

    wrapper.vm.navigate("/users/7");
    await flushPromises();
    expect(wrapper.text()).toContain("User #7");
    expect(wrapper.text()).not.toContain("User #42");
  });
});
