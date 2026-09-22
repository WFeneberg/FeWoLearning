// Exercise 020 — indexed access types (beginner).
// Goal:   name a type that lives inside another type, without repeating it.
// Drills: T["key"], nested lookups, T[number] on an array, T[keyof T].
// Passes: all three derived types match, and the two readers work.
//
// The bracket syntax is the same as a property read, but in type position:
// Order["customer"]["name"] is the type of that field, and Order["lines"]
// [number] is the element type of that array — `number` being the index type
// an array is keyed by. Note that dotted access does NOT work here:
// `Order.customer` is a syntax error in a type, and the brackets are the only
// spelling.
//
// One honest limit, because it shapes how you should read the facts below:
// they assert the resulting TYPE, and nothing can see how you arrived at it.
// Writing `type CustomerName = string` passes just as well as deriving it.
// The discipline is the point, and it pays off the day Order changes — the
// derived type follows, the hand-written one silently does not.

export interface Order {
  id: string;
  customer: { name: string; email: string };
  lines: { sku: string; qty: number }[];
}

/** TODO: the type of the customer's name. Derive it through Order. */
export type CustomerName = unknown;

/** TODO: the element type of Order["lines"]. */
export type OrderLine = unknown;

/** TODO: the union of every value type in Order. */
export type OrderValue = unknown;

/** The total quantity across all lines. */
export function totalQty(_order: Order): number {
  throw new Error("TODO: implement totalQty");
}

/** The line at `index`, or undefined when there is none. */
export function lineAt(_order: Order, _index: number): OrderLine | undefined {
  throw new Error("TODO: implement lineAt");
}
