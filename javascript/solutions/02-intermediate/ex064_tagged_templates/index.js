// Reference solution — exercise 064.

const ESCAPES = { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" };

export function inspect(strings, ...values) {
  // `strings` is a frozen array with a `raw` property hanging off it.
  return { strings: [...strings], values, raw: [...strings.raw] };
}

export function html(strings, ...values) {
  const escape = (value) => String(value).replace(/[&<>"']/g, (char) => ESCAPES[char]);
  return strings.reduce(
    (out, literal, index) => out + literal + (index < values.length ? escape(values[index]) : ""),
    "",
  );
}

export function keepRaw(strings, ...values) {
  return strings.raw.reduce(
    (out, literal, index) => out + literal + (index < values.length ? values[index] : ""),
    "",
  );
}
