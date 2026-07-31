import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import VForKeyList from "./VForKeyList.vue";

describe("VForKeyList", () => {
  it("renders each item as 'index: name'", () => {
    const items = [
      { id: 101, name: "Apple" },
      { id: 102, name: "Banana" },
      { id: 103, name: "Cherry" },
    ];
    const wrapper = mount(VForKeyList, { props: { items } });
    const texts = wrapper.findAll("li").map((li) => li.text());
    expect(texts).toEqual(["0: Apple", "1: Banana", "2: Cherry"]);
  });

  it("uses the item id as the :key so DOM nodes are reused on reorder", async () => {
    const items = [
      { id: 101, name: "Apple" },
      { id: 102, name: "Banana" },
      { id: 103, name: "Cherry" },
    ];
    const wrapper = mount(VForKeyList, { props: { items } });
    const firstLiBefore = wrapper.findAll("li")[0].element;

    const reordered = [items[2], items[0], items[1]];
    await wrapper.setProps({ items: reordered });

    const liElements = wrapper.findAll("li");
    expect(liElements.map((li) => li.text())).toEqual(["0: Cherry", "1: Apple", "2: Banana"]);

    // The <li> for "Apple" (id 101) should be the same DOM node as before,
    // just moved to a new position — proof the :key is bound to the id, not the index.
    const appleLiAfter = liElements[1].element;
    expect(appleLiAfter).toBe(firstLiBefore);
  });
});
