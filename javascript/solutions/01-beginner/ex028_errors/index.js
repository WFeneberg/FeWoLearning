// Reference solution — exercise 028.

export class ValidationError extends Error {
  constructor(message, field) {
    super(message);
    // `name` defaults to "Error" and is what most logging prints.
    this.name = "ValidationError";
    this.field = field;
  }
}

export function validateAge(value) {
  if (typeof value !== "number" || Number.isNaN(value)) {
    throw new ValidationError("age must be a number", "age");
  }
  if (!Number.isInteger(value) || value < 0 || value > 149) {
    throw new ValidationError("age out of range", "age");
  }
  return value;
}

export function attempt(fn, onFinally) {
  try {
    return { ok: true, value: fn() };
  } catch (error) {
    return { ok: false, error };
  } finally {
    onFinally();
  }
}

export function finallyWins() {
  try {
    return "try";
  } finally {
    return "finally";
  }
}
