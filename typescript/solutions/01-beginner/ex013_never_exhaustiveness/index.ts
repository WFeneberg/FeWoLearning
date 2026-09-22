// Reference solution — exercise 013.
export type Shape =
  | { kind: "circle"; radius: number }
  | { kind: "square"; side: number };

export type ShapeV2 = Shape | { kind: "triangle"; base: number; height: number };

export function assertNever(value: never): never {
  throw new Error(`Unexpected value: ${JSON.stringify(value)}`);
}

export function area(shape: Shape): number {
  switch (shape.kind) {
    case "circle":
      return Math.PI * shape.radius ** 2;
    case "square":
      return shape.side ** 2;
    default:
      // Reachable only if Shape gains a member: `shape` is never here today.
      return assertNever(shape);
  }
}
