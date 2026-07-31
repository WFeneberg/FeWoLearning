import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ItemList from "./ItemList.vue";

describe("ItemList", () => {
  it("collects one ref per rendered item, in order", () => {
    const wrapper = mount(ItemList, {
      props: { items: ["alpha", "beta", "gamma"] },
    });

    const vm = wrapper.vm as unknown as { itemRefs: HTMLLIElement[] };
    expect(vm.itemRefs).toHaveLength(3);
    vm.itemRefs.forEach((el) => {
      expect(el).toBeInstanceOf(HTMLLIElement);
    });
    expect(vm.itemRefs.map((el) => el.textContent?.trim())).toEqual([
      "alpha",
      "beta",
      "gamma",
    ]);
  });

  it("re-collects refs (no stale nodes) when the list shrinks", async () => {
    const wrapper = mount(ItemList, {
      props: { items: ["a", "b", "c", "d"] },
    });

    await wrapper.setProps({ items: ["x", "y"] });

    const vm = wrapper.vm as unknown as { itemRefs: HTMLLIElement[] };
    expect(vm.itemRefs).toHaveLength(2);
    vm.itemRefs.forEach((el) => {
      expect(el).toBeInstanceOf(HTMLLIElement);
    });
    expect(
      vm.itemRefs.map((el) => el.textContent?.trim()).sort(),
    ).toEqual(["x", "y"]);
  });

  it("renders an empty ref array for an empty list", () => {
    const wrapper = mount(ItemList, { props: { items: [] } });
    const vm = wrapper.vm as unknown as { itemRefs: HTMLLIElement[] };
    expect(vm.itemRefs).toHaveLength(0);
  });
});
