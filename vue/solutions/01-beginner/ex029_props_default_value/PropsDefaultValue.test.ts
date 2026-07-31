import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import PropsDefaultValue from "./PropsDefaultValue.vue";

describe("PropsDefaultValue", () => {
  it("uses the default size when the prop is omitted", () => {
    const wrapper = mount(PropsDefaultValue);
    expect(wrapper.text()).toBe("medium");
  });

  it("renders the passed size when provided", () => {
    const wrapper = mount(PropsDefaultValue, {
      props: { size: "large" },
    });
    expect(wrapper.text()).toBe("large");
  });
});
