// The declaration file for ./legacy.js.
//
// TODO: replace every `unknown` below with the real type, as read off
// the implementation. Leave the shape of the file alone — the exports
// must stay exactly the ones ./legacy.js has, and in the same forms.
//
// Getting these wrong is worse than having no types at all: a
// declaration file is an UNCHECKED PROMISE about code the compiler
// never looks at, so an error here is silent and travels everywhere the
// module is used. Read the implementation, not the name.
//
// Two details to get right rather than to guess. `separator` has a
// default in the implementation, so it is optional at the call site —
// and an optional parameter, under this track's
// exactOptionalPropertyTypes, is still spelled `separator?: string`
// (ex005, ex009). And DEFAULTS is exported as a plain object, so it is
// mutable; declaring it readonly would be a promise the module does not
// keep.

export declare function slugify(text: unknown, separator?: unknown): unknown;

export declare function truncate(text: unknown, limit: unknown): unknown;

export declare const DEFAULTS: unknown;

declare function format(text: unknown): unknown;
export default format;
