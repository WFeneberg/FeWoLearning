// An untyped JavaScript module, as shipped by some dependency that
// predates TypeScript. Do not change it — that is the premise.

export function slugify(text, separator) {
  const sep = separator === undefined ? "-" : separator;
  return String(text)
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9]+/g, sep)
    .replace(new RegExp(`^\${sep}+|\${sep}+$`, "g"), "");
}

export function truncate(text, limit) {
  return text.length <= limit ? text : `${text.slice(0, limit - 1)}…`;
}

export const DEFAULTS = { separator: "-", limit: 40 };

export default function format(text) {
  return truncate(slugify(text), DEFAULTS.limit);
}
