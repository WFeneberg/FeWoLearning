// Reference solution — exercise 082.
export type Constructor<T = object> = new (...args: any[]) => T;

export class Entity {
  constructor(public readonly id: string) {}

  describe(): string {
    return `entity:${this.id}`;
  }
}

// No return annotation: the returned class expression is anonymous, so
// its type can only be inferred.
export function Timestamped<TBase extends Constructor>(Base: TBase) {
  return class extends Base {
    readonly createdAt: number = Date.now();

    age(now: number): number {
      return now - this.createdAt;
    }
  };
}

export function Taggable<TBase extends Constructor>(Base: TBase) {
  return class extends Base {
    readonly tags: string[] = [];

    addTag(tag: string): this {
      this.tags.push(tag);
      return this;
    }

    hasTag(tag: string): boolean {
      return this.tags.includes(tag);
    }
  };
}
