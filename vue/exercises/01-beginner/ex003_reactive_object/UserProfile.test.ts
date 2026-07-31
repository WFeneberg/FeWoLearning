import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import UserProfile from "./UserProfile.vue";

describe("UserProfile", () => {
  it("renders the initial name and city", () => {
    const wrapper = mount(UserProfile);
    expect(wrapper.find(".name").text()).toBe("Ada");
    expect(wrapper.find(".city").text()).toBe("London");
  });

  it("updates the rendered city after moveTo mutates the nested field", async () => {
    const wrapper = mount(UserProfile);
    wrapper.vm.moveTo("Berlin");
    await wrapper.vm.$nextTick();
    expect(wrapper.find(".city").text()).toBe("Berlin");
    expect(wrapper.find(".name").text()).toBe("Ada");
  });
});
