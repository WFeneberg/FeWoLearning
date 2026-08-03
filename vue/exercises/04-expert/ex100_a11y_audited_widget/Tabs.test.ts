import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import { Tabs, type TabDefinition } from "./Tabs";

const tabs: TabDefinition[] = [
  { id: "one", label: "One", content: "Content one" },
  { id: "two", label: "Two", content: "Content two" },
  { id: "three", label: "Three", content: "Content three" },
];

function mountTabs(props: Record<string, unknown> = {}) {
  return mount(Tabs, {
    props: { tabs, ...props },
    // Focus assertions need the nodes to be in the real document.
    attachTo: document.body,
  });
}

describe("Tabs — ARIA structure", () => {
  it("uses the tablist / tab / tabpanel roles and labels the tablist", () => {
    const wrapper = mountTabs({ label: "Sections" });

    const tablist = wrapper.get('[role="tablist"]');
    expect(tablist.attributes("aria-label")).toBe("Sections");
    expect(wrapper.findAll('[role="tab"]')).toHaveLength(3);
    expect(wrapper.findAll('[role="tabpanel"]')).toHaveLength(1);
  });

  it("wires each tab to its panel in both directions", () => {
    const wrapper = mountTabs();

    const tab = wrapper.findAll('[role="tab"]')[0];
    const panel = wrapper.get('[role="tabpanel"]');

    expect(tab.attributes("id")).toBe("tab-one");
    expect(tab.attributes("aria-controls")).toBe("panel-one");
    expect(panel.attributes("id")).toBe("panel-one");
    expect(panel.attributes("aria-labelledby")).toBe("tab-one");
  });

  it("renders only the active panel", () => {
    const wrapper = mountTabs();

    expect(wrapper.text()).toContain("Content one");
    expect(wrapper.text()).not.toContain("Content two");
  });

  it("makes the panel focusable", () => {
    const wrapper = mountTabs();

    expect(wrapper.get('[role="tabpanel"]').attributes("tabindex")).toBe("0");
  });
});

describe("Tabs — selection and roving tabindex", () => {
  it("selects the first tab by default", () => {
    const wrapper = mountTabs();
    const all = wrapper.findAll('[role="tab"]');

    expect(all.map((t) => t.attributes("aria-selected"))).toEqual(["true", "false", "false"]);
    expect(all.map((t) => t.attributes("tabindex"))).toEqual(["0", "-1", "-1"]);
  });

  it("honours defaultTabId", () => {
    const wrapper = mountTabs({ defaultTabId: "three" });

    expect(wrapper.findAll('[role="tab"]')[2].attributes("aria-selected")).toBe("true");
    expect(wrapper.text()).toContain("Content three");
  });

  it("activates a tab on click and moves the tab stop with it", async () => {
    const wrapper = mountTabs();

    await wrapper.findAll('[role="tab"]')[1].trigger("click");

    const all = wrapper.findAll('[role="tab"]');
    expect(all.map((t) => t.attributes("aria-selected"))).toEqual(["false", "true", "false"]);
    expect(all.map((t) => t.attributes("tabindex"))).toEqual(["-1", "0", "-1"]);
    expect(wrapper.text()).toContain("Content two");
  });

  it("emits change only when the active tab actually changes", async () => {
    const wrapper = mountTabs();

    await wrapper.findAll('[role="tab"]')[1].trigger("click");
    expect(wrapper.emitted("change")).toEqual([["two"]]);

    await wrapper.findAll('[role="tab"]')[1].trigger("click");
    expect(wrapper.emitted("change")).toEqual([["two"]]);
  });
});

describe("Tabs — disabled tabs", () => {
  const withDisabled: TabDefinition[] = [
    { id: "a", label: "A", content: "CA", disabled: true },
    { id: "b", label: "B", content: "CB" },
    { id: "c", label: "C", content: "CC", disabled: true },
    { id: "d", label: "D", content: "CD" },
  ];

  it("starts on the first enabled tab and marks disabled ones", () => {
    const wrapper = mount(Tabs, { props: { tabs: withDisabled }, attachTo: document.body });
    const all = wrapper.findAll('[role="tab"]');

    expect(all[0].attributes("aria-disabled")).toBe("true");
    expect(all[1].attributes("aria-selected")).toBe("true");
    expect(wrapper.text()).toContain("CB");
  });

  it("ignores clicks on a disabled tab", async () => {
    const wrapper = mount(Tabs, { props: { tabs: withDisabled }, attachTo: document.body });

    await wrapper.findAll('[role="tab"]')[2].trigger("click");

    expect(wrapper.findAll('[role="tab"]')[1].attributes("aria-selected")).toBe("true");
    expect(wrapper.emitted("change")).toBeUndefined();
  });

  it("skips disabled tabs when navigating with the arrow keys", async () => {
    const wrapper = mount(Tabs, { props: { tabs: withDisabled }, attachTo: document.body });

    // Active is "b" (index 1); the next enabled one is "d" (index 3).
    await wrapper.findAll('[role="tab"]')[1].trigger("keydown", { key: "ArrowRight" });
    expect(wrapper.findAll('[role="tab"]')[3].attributes("aria-selected")).toBe("true");

    // Wrapping forward from "d" lands back on "b".
    await wrapper.findAll('[role="tab"]')[3].trigger("keydown", { key: "ArrowRight" });
    expect(wrapper.findAll('[role="tab"]')[1].attributes("aria-selected")).toBe("true");
  });
});

describe("Tabs — keyboard navigation", () => {
  it("ArrowRight advances and wraps", async () => {
    const wrapper = mountTabs();
    const at = (i: number) => wrapper.findAll('[role="tab"]')[i];

    await at(0).trigger("keydown", { key: "ArrowRight" });
    expect(at(1).attributes("aria-selected")).toBe("true");

    await at(1).trigger("keydown", { key: "ArrowRight" });
    expect(at(2).attributes("aria-selected")).toBe("true");

    await at(2).trigger("keydown", { key: "ArrowRight" });
    expect(at(0).attributes("aria-selected")).toBe("true");
  });

  it("ArrowLeft goes back and wraps", async () => {
    const wrapper = mountTabs();
    const at = (i: number) => wrapper.findAll('[role="tab"]')[i];

    await at(0).trigger("keydown", { key: "ArrowLeft" });
    expect(at(2).attributes("aria-selected")).toBe("true");
  });

  it("Home and End jump to the ends", async () => {
    const wrapper = mountTabs({ defaultTabId: "two" });
    const at = (i: number) => wrapper.findAll('[role="tab"]')[i];

    await at(1).trigger("keydown", { key: "End" });
    expect(at(2).attributes("aria-selected")).toBe("true");

    await at(2).trigger("keydown", { key: "Home" });
    expect(at(0).attributes("aria-selected")).toBe("true");
  });

  it("moves DOM focus to the newly active tab", async () => {
    const wrapper = mountTabs();

    await wrapper.findAll('[role="tab"]')[0].trigger("keydown", { key: "ArrowRight" });

    expect(document.activeElement).toBe(wrapper.findAll('[role="tab"]')[1].element);
  });

  it("leaves unrelated keys alone", async () => {
    const wrapper = mountTabs();

    await wrapper.findAll('[role="tab"]')[0].trigger("keydown", { key: "a" });

    expect(wrapper.findAll('[role="tab"]')[0].attributes("aria-selected")).toBe("true");
    expect(wrapper.emitted("change")).toBeUndefined();
  });
});
