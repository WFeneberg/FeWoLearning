// Reference solution — exercise 030.
export type Result<T, E> = { ok: true; value: T } | { ok: false; error: E };

// `never` on the unused arm: a success constrains nothing about the error
// type, so it stays open for whatever the caller unions it with.
export function ok<T>(value: T): Result<T, never> {
  return { ok: true, value };
}

export function err<E>(error: E): Result<never, E> {
  return { ok: false, error };
}

export function mapResult<T, E, U>(
  result: Result<T, E>,
  fn: (value: T) => U,
): Result<U, E> {
  return result.ok ? { ok: true, value: fn(result.value) } : result;
}

export function unwrapOr<T, E>(result: Result<T, E>, fallback: T): T {
  return result.ok ? result.value : fallback;
}
