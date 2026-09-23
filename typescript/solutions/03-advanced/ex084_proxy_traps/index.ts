// Reference solution — exercise 084.
export interface Access {
  reads: string[];
}

export function recording<T extends object>(target: T, access: Access): T {
  return new Proxy(target, {
    get(inner, key, receiver): unknown {
      if (typeof key === "string") {
        access.reads.push(key);
      }
      // Reflect, not inner[key]: this is what preserves receiver
      // handling for accessors defined on a prototype.
      return Reflect.get(inner, key, receiver);
    },
  });
}

export function validated<T extends object>(
  target: T,
  isValid: (key: string, value: unknown) => boolean,
): T {
  return new Proxy(target, {
    set(inner, key, value, receiver): boolean {
      if (typeof key === "string" && !isValid(key, value)) {
        throw new TypeError(`rejected write to ${key}`);
      }
      return Reflect.set(inner, key, value, receiver);
    },
  });
}

export function hidingUnderscored<T extends object>(target: T): T {
  return new Proxy(target, {
    has(inner, key): boolean {
      if (typeof key === "string" && key.startsWith("_")) {
        return false;
      }
      return Reflect.has(inner, key);
    },
  });
}
