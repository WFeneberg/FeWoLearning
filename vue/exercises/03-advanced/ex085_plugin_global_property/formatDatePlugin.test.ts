import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import { createApp, defineComponent } from "vue";
import { formatDatePlugin } from "./formatDatePlugin";

// A plain options-API component that relies on `this.$formatDate` being
// injected by the plugin — exactly how app code would use it.
const DateDisplay = defineComponent({
  props: {
    date: { type: [Date, String], required: true },
  },
  template: `<div>{{ $formatDate(date) }}</div>`,
});

describe("formatDatePlugin", () => {
  it("installs $formatDate onto app.config.globalProperties", () => {
    const app = createApp({});
    app.use(formatDatePlugin);
    expect(typeof app.config.globalProperties.$formatDate).toBe("function");
  });

  it("formats a Date object as YYYY-MM-DD via the options API", () => {
    const wrapper = mount(DateDisplay, {
      props: { date: new Date(2024, 0, 15) }, // 15 Jan 2024, built from local parts
      global: { plugins: [formatDatePlugin] },
    });
    expect(wrapper.text()).toBe("2024-01-15");
  });

  it("formats a date-time string, padding single-digit month/day", () => {
    const wrapper = mount(DateDisplay, {
      props: { date: "2023-03-05T00:00:00" },
      global: { plugins: [formatDatePlugin] },
    });
    expect(wrapper.text()).toBe("2023-03-05");
  });

  it("honours a custom separator passed as plugin options", () => {
    const wrapper = mount(DateDisplay, {
      props: { date: new Date(2022, 10, 3) }, // 3 Nov 2022
      global: { plugins: [[formatDatePlugin, { separator: "/" }]] },
    });
    expect(wrapper.text()).toBe("2022/11/03");
  });

  it("does not leak $formatDate onto components that did not receive the plugin", () => {
    const app = createApp({});
    expect(app.config.globalProperties.$formatDate).toBeUndefined();
  });
});
