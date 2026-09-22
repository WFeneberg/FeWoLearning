// Exercise 018 — `keyof` (beginner).
// Goal:   talk about a type's property names as a type of their own.
// Drills: keyof, a type parameter constrained to it, an indexed return.
// Passes: ProductKey is the union of the four names, and getProp reports the
//         property's own type rather than a union of all of them.
//
// `keyof Product` is the union "id" | "name" | "price" | "inStock" — derived,
// so it cannot fall out of step with Product the way a hand-written union
// would. Constraining a type parameter to it is what lets getProp return
// Product[K]: a different type per call, decided by the key literal passed in.
//
// The reflection-free equivalent of C#'s nameof, roughly — except it is a
// type, checked at the call site, and there is nothing at runtime.

export interface Product {
  id: string;
  name: string;
  price: number;
  inStock: boolean;
}

/** TODO: the union of Product's property names. Derive it. */
export type ProductKey = unknown;

/**
 * TODO: read one property, reporting that property's own type.
 * getProp(product, "price") is a number; getProp(product, "name") a string.
 */
export function getProp(_product: Product, _key: unknown): unknown {
  throw new Error("TODO: implement getProp");
}

/** A copy carrying only the listed keys. */
export function pick(_product: Product, _keys: readonly ProductKey[]): Partial<Product> {
  throw new Error("TODO: implement pick");
}
