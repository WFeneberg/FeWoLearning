import { nextTick } from "vue";
import { describe, expect, it } from "vitest";
import { useSettingsWatcher } from "./useSettingsWatcher";

describe("useSettingsWatcher", () => {
  it("starts with a changeCount of 0", () => {
    const { changeCount } = useSettingsWatcher();
    expect(changeCount.value).toBe(0);
  });

  it("increments changeCount when a top-level property is mutated", async () => {
    const { settings, changeCount } = useSettingsWatcher();
    settings.theme = "dark";
    await nextTick();
    expect(changeCount.value).toBe(1);
  });

  it("increments changeCount when a nested property is mutated in place", async () => {
    const { settings, changeCount } = useSettingsWatcher();
    settings.notifications.email = false;
    await nextTick();
    expect(changeCount.value).toBe(1);

    settings.notifications.sms = false;
    await nextTick();
    expect(changeCount.value).toBe(2);
  });
});
