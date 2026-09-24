// Reference solution — exercise 030.

export function parseLogLine(line) {
  const match = line.match(
    /^(?<date>\d{4}-\d{2}-\d{2}) (?<level>[A-Z]+) (?<message>.+)$/,
  );
  return match === null ? null : { ...match.groups };
}

export function parsePairs(text) {
  return [...text.matchAll(/(?<key>\w+)=(?<value>[^\s,]+)/g)].map(({ groups }) => ({
    key: groups.key,
    value: groups.value,
  }));
}

export function reformatDates(text) {
  return text.replace(
    /(?<day>\d{2})\.(?<month>\d{2})\.(?<year>\d{4})/g,
    "$<year>-$<month>-$<day>",
  );
}

export function staleLastIndex() {
  const pattern = /a\d/g;
  return [pattern.test("a1"), pattern.test("a1")];
}
