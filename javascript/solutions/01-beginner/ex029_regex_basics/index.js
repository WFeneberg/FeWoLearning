// Reference solution — exercise 029.

export function isHexColor(text) {
  return /^#(?:[0-9a-f]{3}|[0-9a-f]{6})$/i.test(text);
}

export function firstNumber(text) {
  const match = text.match(/(\d+)/);
  return match === null ? null : Number(match[1]);
}

export function maskDigits(text) {
  return text.replace(/\d/g, "x");
}

export function countWord(text, word) {
  const pattern = new RegExp(`\\b${word}\\b`, "gi");
  return text.match(pattern)?.length ?? 0;
}
