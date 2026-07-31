import { describe, expect, it } from "vitest";
import { defineComponent } from "vue";
import { mount } from "@vue/test-utils";
import Layout from "./Layout.vue";

describe("Layout", () => {
  const items = [
    { id: 1, name: "alpha" },
    { id: 2, name: "beta" },
    { id: 3, name: "gamma" },
  ];

  it("renders one entry per item when no slot is supplied", () => {
    const wrapper = mount(Layout, { props: { items } });
    expect(wrapper.findAll("li").map((li) => li.text())).toEqual([
      "alpha",
      "beta",
      "gamma",
    ]);
  });

  it("passes item and a remove callback through the named 'item' slot", () => {
    const Parent = defineComponent({
      components: { Layout },
      template: `
        <Layout :items="items">
          <template v-slot:item="{ item, remove }">
            <button class="remove-btn" @click="remove">{{ item.name }}</button>
          </template>
        </Layout>
      `,
      data() {
        return { items };
      },
    });

    const wrapper = mount(Parent);
    expect(
      wrapper.findAll(".remove-btn").map((btn) => btn.text()),
    ).toEqual(["alpha", "beta", "gamma"]);
  });

  it("removes the item from the underlying list when the slot's remove callback fires", async () => {
    const Parent = defineComponent({
      components: { Layout },
      template: `
        <Layout :items="items">
          <template v-slot:item="{ item, remove }">
            <button class="remove-btn" @click="remove">{{ item.name }}</button>
          </template>
        </Layout>
      `,
      data() {
        return { items };
      },
    });

    const wrapper = mount(Parent);
    const buttons = wrapper.findAll(".remove-btn");
    await buttons[1].trigger("click");

    expect(
      wrapper.findAll(".remove-btn").map((btn) => btn.text()),
    ).toEqual(["alpha", "gamma"]);
  });

  it("does not mutate the original items prop passed by the parent", async () => {
    const Parent = defineComponent({
      components: { Layout },
      template: `
        <Layout :items="items">
          <template v-slot:item="{ item, remove }">
            <button class="remove-btn" @click="remove">{{ item.name }}</button>
          </template>
        </Layout>
      `,
      data() {
        return { items: items.map((item) => ({ ...item })) };
      },
    });

    const wrapper = mount(Parent);
    await wrapper.findAll(".remove-btn")[0].trigger("click");

    expect((wrapper.vm as unknown as { items: typeof items }).items).toHaveLength(3);
  });
});
