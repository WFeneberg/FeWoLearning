// Reference solution — exercise 025.
export class SoftSecret {
  constructor(private value: string) {}

  reveal(): string {
    return this.value;
  }
}

export class HardSecret {
  #value: string;

  constructor(value: string) {
    this.#value = value;
  }

  reveal(): string {
    return this.#value;
  }

  static isHardSecret(candidate: unknown): candidate is HardSecret {
    // `in` throws on a primitive, so the object check has to come first.
    return typeof candidate === "object" && candidate !== null && #value in candidate;
  }
}

export function visibleFields(instance: object): string[] {
  return Object.keys(instance);
}
