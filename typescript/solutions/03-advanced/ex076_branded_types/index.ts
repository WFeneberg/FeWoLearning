// Reference solution — exercise 076.
declare const brand: unique symbol;

// An intersection, so the branded type is still usable as its base.
export type Brand<T, B extends string> = T & { readonly [brand]: B };

export type UserId = Brand<string, "UserId">;

export type OrderId = Brand<string, "OrderId">;

export function asUserId(value: string): UserId {
  // The one cast the design allows, in the one place that may do it.
  return value as UserId;
}

export function describeUser(id: UserId): string {
  // Still a string: the brand adds a phantom property and takes nothing
  // away.
  return `user:${id.toLowerCase()}`;
}
