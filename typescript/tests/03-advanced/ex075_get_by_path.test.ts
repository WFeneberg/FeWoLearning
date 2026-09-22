import { describe, expect, it } from "vitest";
import { getByPath } from "@ex/03-advanced/ex075_get_by_path/index";
import type { Order } from "@ex/03-advanced/ex075_get_by_path/index";

function order(): Order {
  return {
    id: "o-1",
    customer: {
      name: "Ada",
      email: "ada@example.com",
      address: { city: "Zurich", zip: 8001 },
    },
    total: 42,
  };
}

describe("ex075 getByPath", () => {
  it("reads a top-level value", () => {
    expect(getByPath(order(), "id")).toBe("o-1");
    expect(getByPath(order(), "total")).toBe(42);
  });

  it("reads one level down", () => {
    expect(getByPath(order(), "customer.name")).toBe("Ada");
  });

  it("reads two levels down", () => {
    expect(getByPath(order(), "customer.address.city")).toBe("Zurich");
    expect(getByPath(order(), "customer.address.zip")).toBe(8001);
  });

  it("reads a branch as a whole", () => {
    expect(getByPath(order(), "customer.address")).toEqual({ city: "Zurich", zip: 8001 });
  });
});
