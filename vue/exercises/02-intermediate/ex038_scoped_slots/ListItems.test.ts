import { describe, expect, it } from "vitest";
import { defineComponent } from "vue";
import { mount } from "@vue/test-utils";
import ListItems from "./ListItems.vue";

describe("ListItems", () => {
  it("exposes item and index to a scoped default slot", () => {
    const Parent = defineComponent({
      components: { ListItems },
      template: `
        <ListItems :items="['Cabin', 'Yurt', 'Treehouse']">
          <template v-slot="{ item, index }">
            <span class="entry">{{ index }}:{{ item }}</span>
          </template>
        </ListItems>
      `,
    });

    const wrapper = mount(Parent);
    const entries = wrapper.findAll(".entry").map((el) => el.text());

    expect(entries).toEqual(["0:Cabin", "1:Yurt", "2:Treehouse"]);
  });

  it("renders one <li> per item and updates when the slot content differs per index", () => {
    const Parent = defineComponent({
      components: { ListItems },
      template: `
        <ListItems :items="['A', 'B']">
          <template v-slot="{ item, index }">
            <strong v-if="index === 0">First: {{ item }}</strong>
            <em v-else>Other: {{ item }}</em>
          </template>
        </ListItems>
      `,
    });

    const wrapper = mount(Parent);
    const items = wrapper.findAll("li");

    expect(items).toHaveLength(2);
    expect(items[0].find("strong").text()).toBe("First: A");
    expect(items[1].find("em").text()).toBe("Other: B");
  });
});
