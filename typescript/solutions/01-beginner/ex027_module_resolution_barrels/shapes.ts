// Given, for ex027's barrel to re-export. Do not change.
export interface Shape {
  readonly kind: "circle" | "square";
  readonly size: number;
}

export function describe(shape: Shape): string {
  return `${shape.kind}(${shape.size})`;
}

export function makeCircle(size: number): Shape {
  return { kind: "circle", size };
}
