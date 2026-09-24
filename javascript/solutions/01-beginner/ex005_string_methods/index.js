// Reference solution — exercise 005.

export function normalizeSpaces(text) {
  // split() with no argument splits on every whitespace run and drops the
  // empties at the ends — which is trim and collapse in one call.
  return text.split(/\s+/).filter(Boolean).join(" ");
}

export function padId(id, width) {
  return String(id).padStart(width, "0");
}

export function maskTail(text, needle, replacement) {
  return text.replaceAll(needle, replacement);
}

export function lastChar(text) {
  return text.at(-1);
}

export function mutateFirstChar(text) {
  try {
    // Strict mode: assigning to an index of a string primitive throws.
    text[0] = "X";
  } catch {
    // Expected — a string is a value, not a buffer.
  }
  return text;
}
