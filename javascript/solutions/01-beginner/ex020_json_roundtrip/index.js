// Reference solution — exercise 020.

const ISO = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?Z$/;

export function roundTrip(value) {
  return JSON.parse(JSON.stringify(value));
}

export function redact(value, secretKeys) {
  return JSON.stringify(value, (key, current) =>
    secretKeys.includes(key) ? undefined : current,
  );
}

export function parseWithDates(json) {
  return JSON.parse(json, (_key, value) =>
    typeof value === "string" && ISO.test(value) ? new Date(value) : value,
  );
}

export function stringifyStable(object) {
  // The key allowlist applies at every level, so collect the whole tree's
  // keys once and hand them over sorted.
  const keys = new Set();
  const collect = (value) => {
    if (Array.isArray(value)) {
      value.forEach(collect);
    } else if (value !== null && typeof value === "object") {
      for (const [key, nested] of Object.entries(value)) {
        keys.add(key);
        collect(nested);
      }
    }
  };
  collect(object);
  return JSON.stringify(object, [...keys].sort());
}
