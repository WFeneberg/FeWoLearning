import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ListRendering from "./ListRendering.vue";

describe("ListRendering", () => {
  it("renders one <li> per item", () => {
    const wrapper = mount(ListRendering, { props: { items: ["Apple", "Banana", "Cherry"] } });
    expect(wrapper.findAll("li")).toHaveLength(3);
  });

  it("renders the item text in order", () => {
    const wrapper = mount(ListRendering, { props: { items: ["Apple", "Banana", "Cherry"] } });
    const texts = wrapper.findAll("li").map((li) => li.text());
    expect(texts).toEqual(["Apple", "Banana", "Cherry"]);
  });

  it("renders no <li> for an empty array", () => {
    const wrapper = mount(ListRendering, { props: { items: [] } });
    expect(wrapper.findAll("li")).toHaveLength(0);
  });
});
