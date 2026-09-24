// Reference solution — exercise 003.

export function falsyValues() {
  return [false, 0, -0, 0n, "", null, undefined, NaN];
}

export function orDefault(value, fallback) {
  return value || fallback;
}

export function withDefault(value, fallback) {
  return value ?? fallback;
}

export function applyDefaults(config) {
  config.retries ??= 3;
  config.timeoutMs ??= 1000;
  config.label ??= "job";
  return config;
}
