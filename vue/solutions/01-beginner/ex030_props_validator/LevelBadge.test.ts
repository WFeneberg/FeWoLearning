import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import LevelBadge from "./LevelBadge.vue";

describe("LevelBadge", () => {
  it("renders normally for a valid level", () => {
    const wrapper = mount(LevelBadge, { props: { level: "medium" } });
    expect(wrapper.text()).toBe("medium");
  });

  it("validator accepts only low, medium, high", () => {
    const validator = (LevelBadge as any).props.level.validator as (
      value: string,
    ) => boolean;
    expect(validator("low")).toBe(true);
    expect(validator("medium")).toBe(true);
    expect(validator("high")).toBe(true);
    expect(validator("extreme")).toBe(false);
  });
});
