// Reference solution — exercise 098, the declaration file.
export declare function slugify(text: string, separator?: string): string;

export declare function truncate(text: string, limit: number): string;

// Mutable, because the module exports a plain object. Declaring it
// readonly would be a promise ./legacy.js does not keep.
export declare const DEFAULTS: { separator: string; limit: number };

declare function format(text: string): string;
export default format;
