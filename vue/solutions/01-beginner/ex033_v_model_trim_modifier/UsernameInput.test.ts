import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import UsernameInput from "./UsernameInput.vue";

describe("UsernameInput", () => {
  it("trims leading and trailing whitespace from the input", async () => {
    const wrapper = mount(UsernameInput);
    const input = wrapper.get<HTMLInputElement>('[data-testid="username-input"]');

    await input.setValue("  alice  ");

    const username = (wrapper.vm as unknown as { username: string }).username;
    expect(username).toBe("alice");
  });

  it("keeps interior whitespace intact", async () => {
    const wrapper = mount(UsernameInput);
    const input = wrapper.get<HTMLInputElement>('[data-testid="username-input"]');

    await input.setValue("  jane doe  ");

    const username = (wrapper.vm as unknown as { username: string }).username;
    expect(username).toBe("jane doe");
  });
});
