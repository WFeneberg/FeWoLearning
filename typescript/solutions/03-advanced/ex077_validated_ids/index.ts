// Reference solution — exercise 077.
declare const brand: unique symbol;

type Brand<T, B extends string> = T & { readonly [brand]: B };

export type Email = Brand<string, "Email">;

export type PositiveInt = Brand<number, "PositiveInt">;

function looksLikeEmail(value: string): boolean {
  const parts = value.split("@");
  return (
    parts.length === 2 &&
    parts.every((part) => part.length > 0) &&
    !/\s/.test(value)
  );
}

export function toEmail(value: string): Email | undefined {
  return looksLikeEmail(value) ? (value as Email) : undefined;
}

export function toPositiveInt(value: number): PositiveInt | undefined {
  // Number.isInteger rejects NaN and both infinities on its own.
  return Number.isInteger(value) && value > 0 ? (value as PositiveInt) : undefined;
}

export function assertEmail(value: string): asserts value is Email {
  if (!looksLikeEmail(value)) {
    throw new TypeError(`not an email: ${value}`);
  }
}

export function describeQuota(email: Email, count: PositiveInt): string {
  return `${email} x${count}`;
}
