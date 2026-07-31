import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import Button from "./Button.vue";

describe("Button", () => {
  it("renders the fallback text when no slot content is provided", () => {
    const wrapper = mount(Button);
    expect(wrapper.text()).toBe("Click me");
  });

  it("renders custom slot content when provided", () => {
    const wrapper = mount(Button, {
      slots: {
        default: "Save",
      },
    });
    expect(wrapper.text()).toBe("Save");
  });

  it("still renders as a <button> element with custom content", () => {
    const wrapper = mount(Button, {
      slots: {
        default: "Delete",
      },
    });
    expect(wrapper.find("button").exists()).toBe(true);
    expect(wrapper.find("button").text()).toBe("Delete");
  });
});
