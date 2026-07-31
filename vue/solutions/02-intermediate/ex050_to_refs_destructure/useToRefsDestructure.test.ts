import { defineComponent, h } from "vue";
import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import { useToRefsDestructure } from "./useToRefsDestructure";

function makeHarness() {
  return defineComponent({
    setup() {
      // Destructuring here would lose reactivity if the composable
      // returned a plain `reactive()` object instead of `toRefs()`.
      const { name, age, birthday, rename } = useToRefsDestructure({
        name: "Ada",
        age: 30,
      });
      return { name, age, birthday, rename };
    },
    render() {
      return h("div", [
        h("span", { class: "name" }, this.name),
        h("span", { class: "age" }, String(this.age)),
      ]);
    },
  });
}

describe("useToRefsDestructure", () => {
  it("exposes the initial values through the destructured refs", () => {
    const { name, age } = useToRefsDestructure({ name: "Grace", age: 42 });
    expect(name.value).toBe("Grace");
    expect(age.value).toBe(42);
  });

  it("keeps a destructured ref reactive when the source object changes", () => {
    const { state, name } = useToRefsDestructure({ name: "Grace", age: 42 });
    state.name = "Hopper";
    expect(name.value).toBe("Hopper");
  });

  it("updates the rendered template after a destructured property changes", async () => {
    const wrapper = mount(makeHarness());

    expect(wrapper.find(".name").text()).toBe("Ada");
    expect(wrapper.find(".age").text()).toBe("30");

    await wrapper.vm.birthday();
    expect(wrapper.find(".age").text()).toBe("31");

    await wrapper.vm.rename("Ada Lovelace");
    expect(wrapper.find(".name").text()).toBe("Ada Lovelace");
  });
});
