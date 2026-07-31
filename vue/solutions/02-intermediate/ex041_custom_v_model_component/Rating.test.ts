import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import { defineComponent, ref } from "vue";
import Rating from "./Rating.vue";

describe("Rating", () => {
  it("renders max stars, marking the current rating as filled", () => {
    const wrapper = mount(Rating, { props: { modelValue: 2 } });
    const buttons = wrapper.findAll("button");
    expect(buttons).toHaveLength(5);
    expect(buttons[0].classes()).toContain("filled");
    expect(buttons[1].classes()).toContain("filled");
    expect(buttons[2].classes()).not.toContain("filled");
  });

  it("emits update:modelValue with the clicked star's value", async () => {
    const wrapper = mount(Rating, { props: { modelValue: 1 } });
    await wrapper.get('[data-testid="star-4"]').trigger("click");

    const emitted = wrapper.emitted("update:modelValue");
    expect(emitted).toBeTruthy();
    expect(emitted![0]).toEqual([4]);
  });

  it("supports a custom max", () => {
    const wrapper = mount(Rating, { props: { modelValue: 0, max: 3 } });
    expect(wrapper.findAll("button")).toHaveLength(3);
  });

  it("updates the parent's bound ref via v-model", async () => {
    const parent = defineComponent({
      components: { Rating },
      setup() {
        const rating = ref(2);
        return { rating };
      },
      template: `<Rating v-model="rating" />`,
    });

    const wrapper = mount(parent);
    await wrapper.get('[data-testid="star-5"]').trigger("click");

    expect((wrapper.vm as unknown as { rating: number }).rating).toBe(5);

    const stars = wrapper.findAll("button");
    expect(stars[4].classes()).toContain("filled");
  });
});
