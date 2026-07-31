import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import PropsTypedMessage from "./PropsTypedMessage.vue";

describe("PropsTypedMessage", () => {
  it("renders the message prop verbatim", () => {
    const wrapper = mount(PropsTypedMessage, {
      props: { message: "Hello, Vue!" },
    });
    expect(wrapper.text()).toBe("Hello, Vue!");
  });

  it("re-renders when the prop changes", async () => {
    const wrapper = mount(PropsTypedMessage, {
      props: { message: "First" },
    });
    await wrapper.setProps({ message: "Second" });
    expect(wrapper.text()).toBe("Second");
  });
});
