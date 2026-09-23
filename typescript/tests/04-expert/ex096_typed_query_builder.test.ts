import { describe, expect, it } from "vitest";
import { from } from "@ex/04-expert/ex096_typed_query_builder/index";
import type { Row } from "@ex/04-expert/ex096_typed_query_builder/index";

const rows: Row[] = [
  { id: 1, name: "Ada", email: "ada@example.com", active: true },
  { id: 2, name: "Grace", email: "grace@example.com", active: false },
  { id: 3, name: "Linus", email: "linus@example.com", active: true },
];

describe("ex096 select", () => {
  it("projects onto the chosen columns", () => {
    expect(from<Row>().select("id", "name").run(rows)).toEqual([
      { id: 1, name: "Ada" },
      { id: 2, name: "Grace" },
      { id: 3, name: "Linus" },
    ]);
  });

  // Additive, not replacing: the second call adds to the first.
  it("combines two selects rather than replacing", () => {
    expect(from<Row>().select("id").select("name").run(rows.slice(0, 1))).toEqual([
      { id: 1, name: "Ada" },
    ]);
  });

  it("projects onto nothing when nothing was selected", () => {
    expect(from<Row>().run(rows.slice(0, 1))).toEqual([{}]);
  });
});

describe("ex096 where", () => {
  it("filters on a value", () => {
    expect(from<Row>().select("name").where("active", true).run(rows)).toEqual([
      { name: "Ada" },
      { name: "Linus" },
    ]);
  });

  it("combines filters", () => {
    expect(
      from<Row>().select("id").where("active", true).where("name", "Linus").run(rows),
    ).toEqual([{ id: 3 }]);
  });

  it("can match nothing", () => {
    expect(from<Row>().select("id").where("name", "Nobody").run(rows)).toEqual([]);
  });

  it("works in either order with select", () => {
    expect(from<Row>().where("id", 2).select("name").run(rows)).toEqual([{ name: "Grace" }]);
  });
});
