import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RangeSlider from "./RangeSlider.vue";

describe("RangeSlider", () => {
  it("renders two range inputs reflecting min and max props", () => {
    const wrapper = mount(RangeSlider, { props: { min: 10, max: 90 } });
    const inputs = wrapper.findAll('input[type="range"]');
    expect(inputs).toHaveLength(2);
    expect((inputs[0].element as HTMLInputElement).value).toBe("10");
    expect((inputs[1].element as HTMLInputElement).value).toBe("90");
  });

  it("emits update:min with the new value when the min input changes", async () => {
    const wrapper = mount(RangeSlider, { props: { min: 10, max: 90 } });
    const [minInput] = wrapper.findAll('input[type="range"]');
    await minInput.setValue("25");
    expect(wrapper.emitted("update:min")?.[0]).toEqual([25]);
    expect(wrapper.emitted("update:max")).toBeUndefined();
  });

  it("emits update:max with the new value when the max input changes", async () => {
    const wrapper = mount(RangeSlider, { props: { min: 10, max: 90 } });
    const [, maxInput] = wrapper.findAll('input[type="range"]');
    await maxInput.setValue("75");
    expect(wrapper.emitted("update:max")?.[0]).toEqual([75]);
    expect(wrapper.emitted("update:min")).toBeUndefined();
  });

  it("updates independently across multiple changes", async () => {
    const wrapper = mount(RangeSlider, { props: { min: 0, max: 100 } });
    const [minInput, maxInput] = wrapper.findAll('input[type="range"]');
    await minInput.setValue("5");
    await maxInput.setValue("95");
    await minInput.setValue("15");

    expect(wrapper.emitted("update:min")).toEqual([[5], [15]]);
    expect(wrapper.emitted("update:max")).toEqual([[95]]);
  });
});
