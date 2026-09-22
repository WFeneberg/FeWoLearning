// Reference solution — exercise 053.
export interface Circle {
  radius: number;
}

export interface Square {
  side: number;
}

export type Shape = Circle | Square;

export class ApiError extends Error {
  constructor(
    message: string,
    public readonly status: number,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export class TimeoutError extends ApiError {
  constructor(public readonly afterMs: number) {
    super(`timed out after ${afterMs}ms`, 408);
    this.name = "TimeoutError";
  }
}

export function area(shape: Shape): number {
  if ("radius" in shape) {
    return Math.PI * shape.radius ** 2;
  }
  return shape.side ** 2;
}

export function isCircle(shape: Shape): shape is Circle {
  return "radius" in shape;
}

export function describeThrown(value: unknown): string {
  // Most specific first: instanceof walks the prototype chain, so a
  // TimeoutError is an ApiError is an Error.
  if (value instanceof TimeoutError) {
    return `timeout:${value.afterMs}`;
  }
  if (value instanceof ApiError) {
    return `api:${value.status}`;
  }
  if (value instanceof Error) {
    return `error:${value.message}`;
  }
  return "unknown";
}
