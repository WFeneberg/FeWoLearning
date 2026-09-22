// Reference solution — exercise 047.
export type EventName<K extends string> = `on${K}`;

// A pattern, not a single literal: every "<digits>px" is assignable.
export type Px = `${number}px`;

export type ClassName<V extends string, S extends string> = `${V}-${S}`;

export function className<V extends string, S extends string>(
  variant: V,
  size: S,
): ClassName<V, S> {
  return `${variant}-${size}`;
}
