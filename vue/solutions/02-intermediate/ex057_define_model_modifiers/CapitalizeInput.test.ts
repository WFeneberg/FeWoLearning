import { describe, expect, it } from "vitest";
import { defineComponent, ref } from "vue";
import { mount } from "@vue/test-utils";
import CapitalizeInput from "./CapitalizeInput.vue";

// A tiny parent component that uses v-model.capitalize on CapitalizeInput,
// so we can assert the modifier transforms the value on the way in.
const Parent = defineComponent({
  components: { CapitalizeInput },
  setup() {
    const name = ref("ada");
    return { name };
  },
  template: `
    <div>
      <CapitalizeInput label="Name" v-model.capitalize="name" />
      <p data-testid="mirror">{{ name }}</p>
    </div>
  `,
});

describe("CapitalizeInput", () => {
  it("initializes the child input from the parent's v-model value, unmodified", () => {
    const wrapper = mount(Parent);
    const input = wrapper.get<HTMLInputElement>("[data-testid='input']");
    expect(input.element.value).toBe("ada");
    expect(wrapper.get("[data-testid='mirror']").text()).toBe("ada");
  });

  it("capitalizes the first letter when the modifier is applied", async () => {
    const wrapper = mount(Parent);
    const input = wrapper.get("[data-testid='input']");

    await input.setValue("grace");

    expect(wrapper.get("[data-testid='mirror']").text()).toBe("Grace");
  });

  it("leaves an already-capitalized value untouched", async () => {
    const wrapper = mount(Parent);
    const input = wrapper.get("[data-testid='input']");

    await input.setValue("Hopper");

    expect(wrapper.get("[data-testid='mirror']").text()).toBe("Hopper");
  });

  it("capitalizes single-character input", async () => {
    const wrapper = mount(Parent);
    const input = wrapper.get("[data-testid='input']");

    await input.setValue("z");

    expect(wrapper.get("[data-testid='mirror']").text()).toBe("Z");
  });

  it("renders the label text", () => {
    const wrapper = mount(Parent);
    expect(wrapper.get("[data-testid='label']").text()).toBe("Name");
  });
});
