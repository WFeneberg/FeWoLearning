import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import Greeting from "./Greeting.vue";

describe("Greeting", () => {
  it("renders the greeting from the prop", () => {
    const wrapper = mount(Greeting, { props: { name: "Ada" } });
    expect(wrapper.text()).toContain("Hello, Ada!");
  });

  it("emits greet with the name on button click", async () => {
    const wrapper = mount(Greeting, { props: { name: "Ada" } });
    await wrapper.find("button").trigger("click");
    expect(wrapper.emitted("greet")?.[0]).toEqual(["Ada"]);
  });
});
