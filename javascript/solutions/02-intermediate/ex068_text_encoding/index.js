// Reference solution — exercise 068.

const encoder = new TextEncoder();
const decoder = new TextDecoder();

export function encodeUtf8(text) {
  return encoder.encode(text);
}

export function decodeUtf8(bytes) {
  return decoder.decode(bytes);
}

export function measure(text) {
  return {
    codeUnits: text.length,
    codePoints: [...text].length,
    bytes: encoder.encode(text).length,
  };
}

export function truncateBytes(text, maxBytes) {
  let out = "";
  let used = 0;
  // Iterating the string yields whole code points, so a character is never
  // cut in half.
  for (const character of text) {
    const size = encoder.encode(character).length;
    if (used + size > maxBytes) break;
    out += character;
    used += size;
  }
  return out;
}
