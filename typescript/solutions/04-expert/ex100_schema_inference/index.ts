// Reference solution — exercise 100.
export type Schema =
  | { readonly kind: "string" }
  | { readonly kind: "number" }
  | { readonly kind: "boolean" }
  | { readonly kind: "array"; readonly of: Schema }
  | { readonly kind: "optional"; readonly of: Schema }
  | { readonly kind: "object"; readonly fields: { readonly [key: string]: Schema } };

/** The keys of a field map whose schema is `optional`. */
type OptionalKeys<F> = {
  [K in keyof F]: F[K] extends { kind: "optional" } ? K : never;
}[keyof F];

type RequiredKeys<F> = Exclude<keyof F, OptionalKeys<F>>;

export type Infer<S> = S extends { kind: "string" }
  ? string
  : S extends { kind: "number" }
    ? number
    : S extends { kind: "boolean" }
      ? boolean
      : S extends { kind: "array"; of: infer E }
        ? Infer<E>[]
        : S extends { kind: "optional"; of: infer E }
          ? Infer<E> | undefined
          : S extends { kind: "object"; fields: infer F }
            ? // Two mapped types intersected: a mapped type carries one
              // modifier for all its keys, so required and optional are
              // built apart and joined.
              { [K in RequiredKeys<F>]: Infer<F[K]> } & {
                [K in OptionalKeys<F>]?: Infer<F[K]>;
              }
            : never;

export function matches<S extends Schema>(schema: S, value: unknown): boolean {
  switch (schema.kind) {
    case "string":
      return typeof value === "string";
    case "number":
      return typeof value === "number";
    case "boolean":
      return typeof value === "boolean";
    case "optional":
      return value === undefined || matches(schema.of, value);
    case "array":
      return Array.isArray(value) && value.every((item) => matches(schema.of, item));
    case "object": {
      if (typeof value !== "object" || value === null || Array.isArray(value)) {
        return false;
      }
      const record = value as Record<string, unknown>;
      return Object.entries(schema.fields).every(([key, field]) =>
        // An optional field may be absent entirely; anything else must
        // be present and must match.
        field.kind === "optional" && !(key in record) ? true : matches(field, record[key]),
      );
    }
  }
}

export function parse<S extends Schema>(schema: S, value: unknown): Infer<S> | undefined {
  // `as never`, deliberately. Measured: returning `value as Infer<S>`
  // — and `as unknown as Infer<S>` too — makes the checker compare the
  // result against a RECURSIVE Infer instantiated on a generic S, and
  // that hits ex095's depth limit right here. `never` is assignable to
  // everything, so it asks for no comparison at all.
  return matches(schema, value) ? (value as never) : undefined;
}
