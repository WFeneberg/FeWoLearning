// Reference solution — exercise 078.
export interface Config {
  host: string;
  port: number;
  secure: boolean;
}

export interface Builder<T> {
  // Phantom: never assigned, and the only reason `Builder<{host, port}>`
  // is distinguishable from `Builder<Config>` at all.
  readonly supplied?: T;

  set<K extends keyof Config, V extends Config[K]>(
    key: K,
    value: V,
  ): Builder<T & Record<K, V>>;
  // A precondition rather than a receiver: callable only once T covers
  // every key of Config.
  build(this: Builder<Config>): Config;
}

export function configBuilder(): Builder<unknown> {
  const values: Partial<Config> = {};

  const builder: Builder<unknown> = {
    set(key, value) {
      values[key] = value as Config[typeof key];
      // One object throughout; only its declared type changes down the
      // chain, and the cast is confined to here.
      return builder as never;
    },
    build(): Config {
      return { ...values } as Config;
    },
  };

  return builder;
}
