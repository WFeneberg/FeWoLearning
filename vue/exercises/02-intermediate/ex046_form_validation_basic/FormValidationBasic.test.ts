import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import FormValidationBasic from "./FormValidationBasic.vue";

describe("FormValidationBasic", () => {
  it("shows a required-field error when submitting an empty email", async () => {
    const wrapper = mount(FormValidationBasic);

    expect(wrapper.find('[role="alert"]').exists()).toBe(false);

    await wrapper.find("form").trigger("submit");

    const alert = wrapper.find('[role="alert"]');
    expect(alert.exists()).toBe(true);
    expect(alert.text()).toBe("Email is required");
  });

  it("clears the error once a non-empty value is submitted", async () => {
    const wrapper = mount(FormValidationBasic);

    await wrapper.find("form").trigger("submit");
    expect(wrapper.find('[role="alert"]').exists()).toBe(true);

    await wrapper.find("#email").setValue("jane@example.com");
    await wrapper.find("form").trigger("submit");

    expect(wrapper.find('[role="alert"]').exists()).toBe(false);
  });
});
