// Reference solution — exercise 090.

export function pricesOnly(text) {
  return text.match(/(?<=[$€£])\d+/gu) ?? [];
}

export function wordsNotAfterNo(text) {
  return text.match(/(?<!\bno )\b[a-z]+\b/gi) ?? [];
}

export function tokenize(text) {
  const pattern = /(?<number>\d+)|(?<operator>[+\-*/])/y;
  const tokens = [];
  while (pattern.lastIndex < text.length) {
    const start = pattern.lastIndex;
    const match = pattern.exec(text);
    if (match === null) throw new SyntaxError(`unexpected character at ${start}`);
    tokens.push(
      match.groups.number === undefined
        ? { type: "operator", value: match.groups.operator }
        : { type: "number", value: match.groups.number },
    );
  }
  return tokens;
}

export function countLetters(text) {
  return (text.match(/\p{L}/gu) ?? []).length;
}

export function interpolate(template, values) {
  return template.replace(/\{(?<key>\w+)\}/g, (whole, ...args) => {
    const { key } = args.at(-1);
    return Object.hasOwn(values, key) ? String(values[key]) : whole;
  });
}
