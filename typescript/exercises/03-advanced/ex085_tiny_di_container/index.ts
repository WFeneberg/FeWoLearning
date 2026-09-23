// Exercise 085 — a container with no metadata (advanced).
// Goal:   resolve dependencies by token, with the type following the
//         token and nothing cast at the call site.
// Drills: a phantom-typed token, a token-to-value map as a type
//         parameter, lifetimes.
// Passes: each token resolves to its own type, an unregistered token is
//         rejected at compile time, and a singleton is built once.
//
// Every DI container in .NET leans on reflection: `GetService<IFoo>()`
// works because the runtime still knows what IFoo is. TypeScript's types
// are gone by then, so a container that resolves "by type" cannot exist —
// the decorator-and-metadata libraries that appear to do it are reading
// `emitDecoratorMetadata` output, which is a compile step, not the type
// system.
//
// The alternative is better than the imitation: make the TOKEN carry the
// type. A token is a value with a phantom type parameter (ex076's brand,
// once more), and the container's own type is a map from token to value.
// `resolve` then reads the type out of the token, an unregistered token
// is a compile error rather than a runtime one, and there is no
// reflection anywhere.
//
// The lifetimes are the runtime half, and they are the part people get
// wrong: a TRANSIENT factory runs on every resolve, a SINGLETON runs once
// and caches — and "once" must mean once even when the factory is slow
// or the same token is resolved twice in one expression.
//
// Note the shape of the token type below. The phantom property is
// optional and never assigned, exactly as in ex078, so that two tokens
// for different types are not interchangeable.

declare const tokenType: unique symbol;

/** A key that remembers what it resolves to. */
export interface Token<T> {
  readonly name: string;
  readonly [tokenType]?: T;
}

/** TODO: make a token. The type argument is the caller's choice. */
export function token<T>(_name: string): Token<T> {
  throw new Error("TODO: implement token");
}

export class Container {
  private readonly factories = new Map<string, () => unknown>();
  private readonly singletons = new Map<string, unknown>();

  /** TODO: register a factory that runs on every resolve. */
  register(_key: unknown, _factory: unknown): this {
    throw new Error("TODO: implement register");
  }

  /** TODO: register a factory that runs at most once. */
  registerSingleton(_key: unknown, _factory: unknown): this {
    throw new Error("TODO: implement registerSingleton");
  }

  /** TODO: resolve a token to its own type. Throw an Error naming the
   *  token when nothing is registered for it. */
  resolve(_key: unknown): unknown {
    throw new Error("TODO: implement resolve");
  }
}
