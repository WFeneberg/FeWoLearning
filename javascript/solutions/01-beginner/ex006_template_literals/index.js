// Reference solution — exercise 006.

export function label(name, count) {
  return `${name}: ${count} item${count === 1 ? "" : "s"}`;
}

export function card(title, lines) {
  const body = lines.map((line) => `- ${line}`).join("\n");
  // A template literal keeps real newlines, so the shape of the string is
  // the shape of the source.
  return `${title}
${"-".repeat(title.length)}${body === "" ? "" : `\n${body}`}`;
}

export function interpolate(value) {
  return `value: ${value}`;
}
