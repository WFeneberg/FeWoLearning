import { describe, expect, it } from "vitest";
import {
  averageOrderValue,
  countByStatus,
  namesOfBigOrders,
  totalRevenue,
} from "@ex/01-beginner/ex008_array_pipeline/index.js";

// A factory, not a shared constant: a mutation in one test must not be able
// to reach another.
const orders = () => [
  { id: "A1", customer: "Ada", total: 120.5, status: "shipped" },
  { id: "A2", customer: "Linus", total: 19.5, status: "pending" },
  { id: "A3", customer: "Ada", total: 60, status: "shipped" },
  { id: "A4", customer: "Grace", total: 200, status: "cancelled" },
];

describe("ex008 totalRevenue", () => {
  it("sums the totals", () => {
    expect(totalRevenue(orders())).toBe(400);
  });

  it("is 0 for an empty list", () => {
    expect(totalRevenue([])).toBe(0);
  });

  it("does not touch the input", () => {
    const input = orders();
    totalRevenue(input);
    expect(input).toEqual(orders());
  });
});

describe("ex008 namesOfBigOrders", () => {
  it("keeps order and duplicates", () => {
    expect(namesOfBigOrders(orders(), 60)).toEqual(["Ada", "Ada", "Grace"]);
  });

  it("includes the boundary value", () => {
    expect(namesOfBigOrders(orders(), 200)).toEqual(["Grace"]);
  });

  it("can match nothing", () => {
    expect(namesOfBigOrders(orders(), 1000)).toEqual([]);
  });
});

describe("ex008 countByStatus", () => {
  it("counts each status that occurs", () => {
    expect(countByStatus(orders())).toEqual({ shipped: 2, pending: 1, cancelled: 1 });
  });

  it("omits statuses that do not occur", () => {
    expect(Object.keys(countByStatus([{ status: "pending", total: 1 }]))).toEqual(["pending"]);
  });

  it("is an empty object for an empty list", () => {
    expect(countByStatus([])).toEqual({});
  });
});

describe("ex008 averageOrderValue", () => {
  it("is the mean", () => {
    expect(averageOrderValue(orders())).toBe(100);
  });

  it("is null rather than NaN for an empty list", () => {
    expect(averageOrderValue([])).toBeNull();
  });
});
