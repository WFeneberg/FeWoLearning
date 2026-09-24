// Reference solution — exercise 067.

export function currencyParts(value, locale, currency) {
  return new Intl.NumberFormat(locale, { style: "currency", currency }).formatToParts(value);
}

export function groupSeparator(locale) {
  return new Intl.NumberFormat(locale)
    .formatToParts(1_000_000)
    .find((part) => part.type === "group").value;
}

export function dateFields(date, locale, timeZone) {
  const parts = new Intl.DateTimeFormat(locale, {
    timeZone,
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).formatToParts(date);
  const pick = (type) => parts.find((part) => part.type === type).value;
  return { year: pick("year"), month: pick("month"), day: pick("day") };
}

export function relative(value, unit, locale) {
  return new Intl.RelativeTimeFormat(locale, { numeric: "always" }).format(value, unit);
}

export function pluralCategory(value, locale) {
  return new Intl.PluralRules(locale).select(value);
}
