import { describe, expect, it } from "vitest";
import { defineComponent, ref } from "vue";
import { mount } from "@vue/test-utils";
import LabeledInput from "./LabeledInput.vue";

// A tiny parent component that uses v-model on LabeledInput, so we can
// assert the two-way binding works from both directions.
const Parent = defineComponent({
  components: { LabeledInput },
  setup() {
    const name = ref("Ada");
    return { name };
  },
  template: `
    <div>
      <LabeledInput label="Name" v-model="name" />
      <p data-testid="mirror">{{ name }}</p>
    </div>
  `,
});

describe("LabeledInput", () => {
  it("initializes the child input from the parent's v-model value", () => {
    const wrapper = mount(Parent);
    const input = wrapper.get<HTMLInputElement>("[data-testid='input']");
    expect(input.element.value).toBe("Ada");
    expect(wrapper.get("[data-testid='mirror']").text()).toBe("Ada");
  });

  it("propagates input changes up to the parent's bound ref", async () => {
    const wrapper = mount(Parent);
    const input = wrapper.get("[data-testid='input']");

    await input.setValue("Grace");

    expect(wrapper.get("[data-testid='mirror']").text()).toBe("Grace");
  });

  it("propagates parent updates back down into the child input", async () => {
    const wrapper = mount(Parent);

    await wrapper.setProps({}); // no-op to ensure component is settled
    (wrapper.vm as unknown as { name: string }).name = "Grace Hopper";
    await wrapper.vm.$nextTick();

    const input = wrapper.get<HTMLInputElement>("[data-testid='input']");
    expect(input.element.value).toBe("Grace Hopper");
  });

  it("renders the label text", () => {
    const wrapper = mount(Parent);
    expect(wrapper.get("[data-testid='label']").text()).toBe("Name");
  });
});
