import { expectTypeOf, test } from "vitest";
import { from } from "@ex/04-expert/ex096_typed_query_builder/index";
import type { Row } from "@ex/04-expert/ex096_typed_query_builder/index";

const rows: Row[] = [];

test("run reports exactly the selected columns", () => {
  expectTypeOf(from<Row>().select("id", "name").run(rows)).toEqualTypeOf<
    Pick<Row, "id" | "name">[]
  >();
});

test("two selects union rather than the second winning", () => {
  expectTypeOf(from<Row>().select("id").select("email").run(rows)).toEqualTypeOf<
    Pick<Row, "id" | "email">[]
  >();
});

// Two facts were written and dropped after measuring them green on the
// untouched tree: "selecting nothing projects onto nothing", which the
// stub's `run(rows): Pick<T, Selected>[]` already satisfies with
// Selected = never, and "a column that does not exist is rejected",
// which its `(keyof T)[]` parameter already does. Both signatures have
// to be given for the tests to chain at all.

test("where does not change the selection", () => {
  expectTypeOf(from<Row>().select("name").where("active", true).run(rows)).toEqualTypeOf<
    Pick<Row, "name">[]
  >();
});

// T[K] in a parameter position: checked against the column named in
// the same call, not against the union of every column type (ex049).
test("a value of the wrong type for its column is rejected", () => {
  // @ts-expect-error — id is a number, however many string columns Row has
  from<Row>().where("id", "1");
});
