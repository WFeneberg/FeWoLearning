import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import VirtualizedListBasic from "./VirtualizedListBasic.vue";

const items = Array.from({ length: 1000 }, (_, i) => `Item ${i}`);

function renderedIndices(wrapper: ReturnType<typeof mount>): number[] {
  return wrapper
    .findAll("[data-index]")
    .map((w) => Number(w.attributes("data-index")))
    .sort((a, b) => a - b);
}

async function scrollTo(wrapper: ReturnType<typeof mount>, scrollTop: number) {
  const viewport = wrapper.get('[data-testid="viewport"]').element as HTMLElement;
  viewport.scrollTop = scrollTop;
  await wrapper.get('[data-testid="viewport"]').trigger("scroll");
}

describe("VirtualizedListBasic", () => {
  it("renders only the initial visible window, not the whole list", () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100 },
    });
    expect(renderedIndices(wrapper)).toEqual([0, 1, 2, 3, 4]);
    expect(wrapper.findAll("[data-index]")).toHaveLength(5);
  });

  it("shifts the rendered window when scrolled down", async () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100 },
    });
    await scrollTo(wrapper, 240);
    expect(renderedIndices(wrapper)).toEqual([12, 13, 14, 15, 16]);
  });

  it("excludes items far outside the visible window", async () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100 },
    });
    await scrollTo(wrapper, 240);
    const indices = renderedIndices(wrapper);
    expect(indices).not.toContain(0);
    expect(indices).not.toContain(999);
    expect(indices).not.toContain(11);
    expect(indices).not.toContain(17);
  });

  it("renders the correct item text at the scrolled position", async () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100 },
    });
    await scrollTo(wrapper, 240);
    expect(wrapper.text()).toContain("Item 12");
    expect(wrapper.text()).toContain("Item 16");
    expect(wrapper.text()).not.toContain("Item 11 ");
  });

  it("pads the window with overscan on both sides", async () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100, overscan: 2 },
    });
    await scrollTo(wrapper, 240);
    expect(renderedIndices(wrapper)).toEqual([10, 11, 12, 13, 14, 15, 16, 17, 18]);
  });

  it("clamps the overscan window at the start of the list", () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100, overscan: 5 },
    });
    expect(renderedIndices(wrapper)).toEqual([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]);
  });

  it("positions rendered rows absolutely by index * itemHeight", () => {
    const wrapper = mount(VirtualizedListBasic, {
      props: { items, itemHeight: 20, viewportHeight: 100 },
    });
    const third = wrapper.get('[data-index="2"]');
    expect(third.attributes("style")).toContain("top: 40px");
  });
});
