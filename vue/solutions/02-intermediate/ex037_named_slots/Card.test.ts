import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import Card from "./Card.vue";

describe("Card", () => {
  it("renders header, default, and footer slots in DOM order", () => {
    const wrapper = mount(Card, {
      slots: {
        header: "<h2>Reservation</h2>",
        default: "<p>Cabin available in July.</p>",
        footer: "<button>Book now</button>",
      },
    });

    const card = wrapper.find(".card");
    expect(card.find("h2").text()).toBe("Reservation");
    expect(card.find("p").text()).toBe("Cabin available in July.");
    expect(card.find("button").text()).toBe("Book now");

    // Assert relative DOM ordering: header before default content before footer.
    const html = card.html();
    const headerIndex = html.indexOf("Reservation");
    const bodyIndex = html.indexOf("Cabin available");
    const footerIndex = html.indexOf("Book now");
    expect(headerIndex).toBeGreaterThanOrEqual(0);
    expect(headerIndex).toBeLessThan(bodyIndex);
    expect(bodyIndex).toBeLessThan(footerIndex);
  });

  it("omits slot wrappers gracefully when a slot is not provided", () => {
    const wrapper = mount(Card, {
      slots: {
        default: "<p>Only body content.</p>",
      },
    });

    expect(wrapper.find("h2").exists()).toBe(false);
    expect(wrapper.find("p").text()).toBe("Only body content.");
  });
});
