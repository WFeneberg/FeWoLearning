// Reference solution — exercise 010.

export function sortNumbers(values) {
  // toSorted copies; sort would reorder the caller's array.
  return values.toSorted((a, b) => a - b);
}

export function sortStaff(people) {
  return people.toSorted((a, b) => {
    if (a.dept !== b.dept) return a.dept < b.dept ? -1 : 1;
    return b.score - a.score; // stability handles the remaining ties
  });
}

export function sortNames(names, locale) {
  return names.toSorted((a, b) => a.localeCompare(b, locale));
}
