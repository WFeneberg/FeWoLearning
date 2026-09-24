// Exercise 008 — map / filter / reduce (beginner).
// Goal:   express a report as a pipeline instead of a loop with counters.
// Drills: map, filter, reduce with an object accumulator, the empty-array
//         case reduce is famous for.
// Passes: four reports over the same order list, none of which mutates it.
//
// An order looks like:
//   { id: "A1", customer: "Ada", total: 120.5, status: "shipped" }

/** Sum of every order's total. Returns 0 for an empty list. TODO. */
export function totalRevenue(_orders) {
  throw new Error("TODO: implement totalRevenue");
}

/**
 * The customer names of the orders whose total is >= `min`, in order,
 * duplicates included.
 *
 * TODO: implement with filter + map.
 */
export function namesOfBigOrders(_orders, _min) {
  throw new Error("TODO: implement namesOfBigOrders");
}

/**
 * Counts orders per status: { shipped: 2, pending: 1 }. Statuses that do not
 * occur must not appear as keys.
 *
 * TODO: implement with reduce and an object accumulator.
 */
export function countByStatus(_orders) {
  throw new Error("TODO: implement countByStatus");
}

/**
 * Mean order total, or null for an empty list — `reduce` on an empty array
 * with no initial value throws, and dividing by zero would quietly give NaN.
 *
 * TODO: implement.
 */
export function averageOrderValue(_orders) {
  throw new Error("TODO: implement averageOrderValue");
}
