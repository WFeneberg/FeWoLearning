import { describe, expect, it } from "vitest";
import { html, inspect, keepRaw } from "@ex/02-intermediate/ex064_tagged_templates/index.js";

describe("ex064 inspect", () => {
  it("splits the literal parts from the values", () => {
    const result = inspect`a${1}b${2}c`;
    expect(result.strings).toEqual(["a", "b", "c"]);
    expect(result.values).toEqual([1, 2]);
  });

  it("always has one more string than values, empty ones included", () => {
    const result = inspect`${1}`;
    expect(result.strings).toEqual(["", ""]);
    expect(result.values).toEqual([1]);
  });

  it("works with no interpolation at all", () => {
    const result = inspect`plain`;
    expect(result.strings).toEqual(["plain"]);
    expect(result.values).toEqual([]);
  });

  it("keeps the raw text beside the cooked text", () => {
    const result = inspect`line\nbreak`;
    expect(result.strings[0]).toBe("line\nbreak");
    expect(result.raw[0]).toBe("line\\nbreak");
    expect(result.raw[0]).toHaveLength(11);
  });
});

describe("ex064 html", () => {
  it("escapes an interpolated value", () => {
    expect(html`<p>${"<script>"}</p>`).toBe("<p>&lt;script&gt;</p>");
  });

  it("leaves the literal markup alone", () => {
    // The distinction that makes a tag worth using: the template author's
    // own markup survives, the caller's data does not.
    expect(html`<b>bold</b>`).toBe("<b>bold</b>");
  });

  it("escapes all five characters", () => {
    expect(html`${`&<>"'`}`).toBe("&amp;&lt;&gt;&quot;&#39;");
  });

  it("escapes every value, in order", () => {
    expect(html`${"<a>"} and ${"<b>"}`).toBe("&lt;a&gt; and &lt;b&gt;");
  });

  it("stringifies non-strings", () => {
    expect(html`n=${42}`).toBe("n=42");
    expect(html`n=${null}`).toBe("n=null");
  });
});

describe("ex064 keepRaw", () => {
  it("keeps escape sequences as source text", () => {
    expect(keepRaw`a\nb`).toBe("a\\nb");
    expect(keepRaw`a\nb`).toBe(String.raw`a\nb`);
  });

  it("still interleaves the values", () => {
    expect(keepRaw`\t${1}\t${2}`).toBe("\\t1\\t2");
  });

  it("handles a template with no escapes", () => {
    expect(keepRaw`plain ${1}`).toBe("plain 1");
  });
});
