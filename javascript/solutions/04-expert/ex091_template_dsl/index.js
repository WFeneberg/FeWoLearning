// Reference solution — exercise 091.

const FRAGMENT = Symbol("fragment");
const RAW = Symbol("raw");

const isFragment = (value) => value?.[FRAGMENT] === true;
const isRaw = (value) => value?.[RAW] === true;

/** Renumbers a fragment's placeholders so they start after `offset`. */
function shift(text, offset) {
  return text.replace(/\$(\d+)/g, (_whole, index) => `$${Number(index) + offset}`);
}

function fragment(text, values) {
  return { [FRAGMENT]: true, text, values };
}

export function sql(strings, ...values) {
  let text = "";
  const collected = [];
  strings.forEach((literal, index) => {
    text += literal;
    if (index >= values.length) return;
    const value = values[index];
    if (isRaw(value)) {
      text += value.text;
    } else if (isFragment(value)) {
      text += shift(value.text, collected.length);
      collected.push(...value.values);
    } else {
      collected.push(value);
      text += `$${collected.length}`;
    }
  });
  return fragment(text, collected);
}

export function raw(text) {
  return { [RAW]: true, text };
}

export function join(fragments, separator) {
  let text = "";
  const values = [];
  fragments.forEach((part, index) => {
    if (index > 0) text += separator;
    text += shift(part.text, values.length);
    values.push(...part.values);
  });
  return fragment(text, values);
}
