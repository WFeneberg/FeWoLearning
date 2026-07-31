import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import ToRefSingle from "./ToRefSingle.vue";

describe("ToRefSingle", () => {
  it("renders the doubled value derived from the count prop", () => {
    const wrapper = mount(ToRefSingle, { props: { count: 2 } });
    expect(wrapper.get('[data-testid="count"]').text()).toBe("2");
    expect(wrapper.get('[data-testid="doubled"]').text()).toBe("4");
  });

  it("reflects updates to the parent's prop through the toRef", async () => {
    const wrapper = mount(ToRefSingle, { props: { count: 2 } });
    await wrapper.setProps({ count: 5 });
    expect(wrapper.get('[data-testid="count"]').text()).toBe("5");
    expect(wrapper.get('[data-testid="doubled"]').text()).toBe("10");
  });

  it("keeps updating across multiple prop changes", async () => {
    const wrapper = mount(ToRefSingle, { props: { count: 0 } });
    await wrapper.setProps({ count: 10 });
    expect(wrapper.get('[data-testid="doubled"]').text()).toBe("20");
    await wrapper.setProps({ count: -3 });
    expect(wrapper.get('[data-testid="doubled"]').text()).toBe("-6");
  });
});
