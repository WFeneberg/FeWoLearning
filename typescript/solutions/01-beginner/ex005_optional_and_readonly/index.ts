// Reference solution — exercise 005.
export interface Settings {
  readonly id: string;
  theme?: "light" | "dark";
  nickname: string | undefined;
}

// `-?` strips optionality so every key is visited; Pick<Settings, K> is then
// an object whose single property is optional exactly when K was optional,
// and `{} extends …` is true only for such an object.
export type OmittableKeys = {
  [K in keyof Settings]-?: {} extends Pick<Settings, K> ? K : never;
}[keyof Settings];

export function resolveTheme(settings: Settings): "light" | "dark" {
  return settings.theme ?? "light";
}
