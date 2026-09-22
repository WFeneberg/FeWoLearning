// Reference solution — exercise 018.
export interface Product {
  id: string;
  name: string;
  price: number;
  inStock: boolean;
}

export type ProductKey = keyof Product;

export function getProp<K extends ProductKey>(product: Product, key: K): Product[K] {
  return product[key];
}

export function pick(product: Product, keys: readonly ProductKey[]): Partial<Product> {
  const result: Partial<Product> = {};
  for (const key of keys) {
    // The assignment needs the cast: TypeScript checks result[key] against the
    // union of all value types, and cannot see that both sides share one key.
    (result as Record<ProductKey, unknown>)[key] = product[key];
  }
  return result;
}
