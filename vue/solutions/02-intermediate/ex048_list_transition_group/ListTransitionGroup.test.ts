import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ListTransitionGroup from "./ListTransitionGroup.vue";

describe("ListTransitionGroup", () => {
  it("renders one list item per initial entry", () => {
    const wrapper = mount(ListTransitionGroup);
    expect(wrapper.findAll('[data-testid="list-item"]')).toHaveLength(3);
  });

  it("renders a TransitionGroup wrapper named 'list'", () => {
    const wrapper = mount(ListTransitionGroup);
    const transitionGroup = wrapper.findComponent({ name: "TransitionGroup" });
    expect(transitionGroup.exists()).toBe(true);
    expect(transitionGroup.props("name")).toBe("list");
  });

  it("removes one rendered item when it is removed from the underlying array", async () => {
    const wrapper = mount(ListTransitionGroup);
    expect(wrapper.findAll('[data-testid="list-item"]')).toHaveLength(3);

    await wrapper.find('[aria-label="remove-2"]').trigger("click");

    const remaining = wrapper.findAll('[data-testid="list-item"]');
    expect(remaining).toHaveLength(2);
    expect(wrapper.text()).toContain("Alpha");
    expect(wrapper.text()).not.toContain("Bravo");
    expect(wrapper.text()).toContain("Charlie");
  });
});
