import { describe, expect, it } from "vitest";
import { defineComponent } from "vue";
import { mount } from "@vue/test-utils";
import DataTable from "./DataTable.vue";

describe("DataTable", () => {
  const columns = [
    { key: "name", label: "Name" },
    { key: "city", label: "City" },
  ];
  const rows = [
    { name: "ada", city: "london" },
    { name: "grace", city: "new york" },
  ];

  it("renders column headers", () => {
    const wrapper = mount(DataTable, { props: { columns, rows } });
    const headers = wrapper.findAll("th").map((th) => th.text());
    expect(headers).toEqual(["Name", "City"]);
  });

  it("falls back to the plain value when no cell slot is supplied", () => {
    const wrapper = mount(DataTable, { props: { columns, rows } });
    const cells = wrapper.findAll("td").map((td) => td.text());
    expect(cells).toEqual(["ada", "london", "grace", "new york"]);
  });

  it("lets the parent customize cell rendering via the scoped slot", () => {
    const Parent = defineComponent({
      components: { DataTable },
      template: `
        <DataTable :columns="columns" :rows="rows">
          <template v-slot:cell="{ column, value }">
            <strong v-if="column.key === 'name'" class="upper">{{ String(value).toUpperCase() }}</strong>
            <span v-else class="plain">{{ value }}</span>
          </template>
        </DataTable>
      `,
      data() {
        return { columns, rows };
      },
    });

    const wrapper = mount(Parent);
    const uppered = wrapper.findAll(".upper").map((el) => el.text());
    const plain = wrapper.findAll(".plain").map((el) => el.text());

    expect(uppered).toEqual(["ADA", "GRACE"]);
    expect(plain).toEqual(["london", "new york"]);
  });

  it("exposes row and column alongside value in the scoped slot", () => {
    const Parent = defineComponent({
      components: { DataTable },
      template: `
        <DataTable :columns="columns" :rows="rows">
          <template v-slot:cell="{ row, column, value }">
            <span class="combo">{{ column.key }}={{ value }}@{{ row.name }}</span>
          </template>
        </DataTable>
      `,
      data() {
        return { columns, rows };
      },
    });

    const wrapper = mount(Parent);
    const combos = wrapper.findAll(".combo").map((el) => el.text());

    expect(combos).toEqual([
      "name=ada@ada",
      "city=london@ada",
      "name=grace@grace",
      "city=new york@grace",
    ]);
  });
});
