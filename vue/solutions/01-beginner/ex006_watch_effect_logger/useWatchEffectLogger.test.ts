import { describe, expect, it } from "vitest";
import { nextTick } from "vue";
import { useWatchEffectLogger } from "./useWatchEffectLogger";

describe("useWatchEffectLogger", () => {
  it("logs the initial values immediately", () => {
    const { log } = useWatchEffectLogger("Anna", 30);
    expect(log).toEqual(["Anna is 30"]);
  });

  it("logs again when the name ref changes", async () => {
    const { name, log } = useWatchEffectLogger("Anna", 30);
    name.value = "Ben";
    await nextTick();
    expect(log).toEqual(["Anna is 30", "Ben is 30"]);
  });

  it("logs again when the age ref changes", async () => {
    const { age, log } = useWatchEffectLogger("Anna", 30);
    age.value = 31;
    await nextTick();
    expect(log).toEqual(["Anna is 30", "Anna is 31"]);
  });

  it("logs once per change for independent updates to each ref", async () => {
    const { name, age, log } = useWatchEffectLogger("Anna", 30);
    name.value = "Ben";
    await nextTick();
    age.value = 31;
    await nextTick();
    expect(log).toEqual(["Anna is 30", "Ben is 30", "Ben is 31"]);
  });
});
