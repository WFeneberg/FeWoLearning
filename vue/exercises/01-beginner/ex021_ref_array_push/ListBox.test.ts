import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ListBox from "./ListBox.vue";

describe("ListBox", () => {
  it("renders one <li> per initial item", () => {
    const wrapper = mount(ListBox);
    expect(wrapper.findAll("li")).toHaveLength(2);
  });

  it("adds one more rendered <li> with the new content after clicking Add", async () => {
    const wrapper = mount(ListBox);
    await wrapper.find("button").trigger("click");
    const items = wrapper.findAll("li");
    expect(items).toHaveLength(3);
    expect(items[2].text()).toBe("Milk");
  });

  it("pushes into the underlying list ref", async () => {
    const wrapper = mount(ListBox);
    await wrapper.find("button").trigger("click");
    expect(wrapper.vm.list).toEqual(["Apples", "Bread", "Milk"]);
  });
});
