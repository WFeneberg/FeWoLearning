import { describe, expect, it } from "vitest";
import { join, raw, sql } from "@ex/04-expert/ex091_template_dsl/index.js";

describe("ex091 sql", () => {
  it("numbers the placeholders", () => {
    const query = sql`SELECT * FROM t WHERE id = ${7} AND name = ${"ada"}`;
    expect(query.text).toBe("SELECT * FROM t WHERE id = $1 AND name = $2");
    expect(query.values).toEqual([7, "ada"]);
  });

  it("works with no values at all", () => {
    expect(sql`SELECT 1`).toMatchObject({ text: "SELECT 1", values: [] });
  });

  it("keeps an injection attempt out of the text", () => {
    // The row's reason to exist. The attacker's string is data.
    const attack = "'; DROP TABLE users; --";
    const query = sql`SELECT * FROM t WHERE name = ${attack}`;
    expect(query.text).toBe("SELECT * FROM t WHERE name = $1");
    expect(query.text).not.toContain("DROP");
    expect(query.values).toEqual([attack]);
  });

  it("keeps a value that looks like a placeholder as a value", () => {
    const query = sql`WHERE a = ${"$1"} AND b = ${"x"}`;
    expect(query.text).toBe("WHERE a = $1 AND b = $2");
    expect(query.values).toEqual(["$1", "x"]);
  });

  it("passes null and undefined through as values", () => {
    const query = sql`SET a = ${null}, b = ${undefined}`;
    expect(query.text).toBe("SET a = $1, b = $2");
    expect(query.values).toEqual([null, undefined]);
  });
});

describe("ex091 composition", () => {
  it("inlines a fragment and renumbers it", () => {
    const where = sql`name = ${"ada"} AND age > ${30}`;
    const query = sql`SELECT * FROM t WHERE ${where} LIMIT ${10}`;
    expect(query.text).toBe("SELECT * FROM t WHERE name = $1 AND age > $2 LIMIT $3");
    expect(query.values).toEqual(["ada", 30, 10]);
  });

  it("renumbers a fragment that comes after a value", () => {
    const where = sql`id = ${1}`;
    const query = sql`SELECT ${"col"} FROM t WHERE ${where}`;
    expect(query.text).toBe("SELECT $1 FROM t WHERE id = $2");
    expect(query.values).toEqual(["col", 1]);
  });

  it("nests two levels deep", () => {
    const inner = sql`a = ${1}`;
    const middle = sql`(${inner} AND b = ${2})`;
    const outer = sql`WHERE ${middle} OR c = ${3}`;
    expect(outer.text).toBe("WHERE (a = $1 AND b = $2) OR c = $3");
    expect(outer.values).toEqual([1, 2, 3]);
  });
});

describe("ex091 raw", () => {
  it("splices literal text with no placeholder", () => {
    const query = sql`SELECT * FROM ${raw("users")} WHERE id = ${1}`;
    expect(query.text).toBe("SELECT * FROM users WHERE id = $1");
    expect(query.values).toEqual([1]);
  });

  it("is the only way to reach the text, and it is explicit", () => {
    const query = sql`ORDER BY ${"name DESC"}`;
    expect(query.text).toBe("ORDER BY $1");
    expect(sql`ORDER BY ${raw("name DESC")}`.text).toBe("ORDER BY name DESC");
  });
});

describe("ex091 join", () => {
  it("joins fragments and renumbers them", () => {
    const parts = [sql`a = ${1}`, sql`b = ${2}`, sql`c = ${3}`];
    const joined = join(parts, " AND ");
    expect(joined.text).toBe("a = $1 AND b = $2 AND c = $3");
    expect(joined.values).toEqual([1, 2, 3]);
  });

  it("is empty for no fragments", () => {
    expect(join([], ", ")).toMatchObject({ text: "", values: [] });
  });

  it("produces a fragment that composes further", () => {
    const joined = join([sql`a = ${1}`, sql`b = ${2}`], " AND ");
    const query = sql`SELECT * FROM t WHERE ${joined} LIMIT ${5}`;
    expect(query.text).toBe("SELECT * FROM t WHERE a = $1 AND b = $2 LIMIT $3");
    expect(query.values).toEqual([1, 2, 5]);
  });
});
