// Reference solution — exercise 085.
declare const tokenType: unique symbol;

export interface Token<T> {
  readonly name: string;
  readonly [tokenType]?: T;
}

export function token<T>(name: string): Token<T> {
  // The phantom is never assigned; only the name exists at runtime.
  return { name };
}

export class Container {
  private readonly factories = new Map<string, () => unknown>();
  private readonly singletons = new Map<string, unknown>();

  register<T>(key: Token<T>, factory: () => T): this {
    this.factories.set(key.name, factory);
    this.singletons.delete(key.name);
    return this;
  }

  registerSingleton<T>(key: Token<T>, factory: () => T): this {
    let built = false;
    let value: T;
    this.factories.set(key.name, () => {
      if (!built) {
        value = factory();
        built = true;
      }
      return value;
    });
    return this;
  }

  // The return type is read out of the token, so nothing is cast at the
  // call site.
  resolve<T>(key: Token<T>): T {
    const factory = this.factories.get(key.name);
    if (factory === undefined) {
      throw new Error(`nothing registered for ${key.name}`);
    }
    return factory() as T;
  }
}
