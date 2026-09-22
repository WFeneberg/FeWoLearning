import { expectTypeOf, test } from "vitest";
import type {
  MyPartial,
  MyRequired,
  PartialBy,
  Row,
} from "@ex/02-intermediate/ex041_rebuild_partial_required/index";

/** Which keys of T may be left out entirely — the ex005 probe. */
type OptionalKeys<T> = {
  [K in keyof T]-?: {} extends Pick<T, K> ? K : never;
}[keyof T];

type RequiredKeys<T> = {
  [K in keyof T]-?: {} extends Pick<T, K> ? never : K;
}[keyof T];

test("MyPartial makes everything optional and keeps readonly", () => {
  expectTypeOf<MyPartial<Row>>().toEqualTypeOf<{
    readonly id?: string;
    label?: string;
    count?: number;
  }>();
});

// `-?` strips `| undefined` from the value as well as removing the marker,
// which is why label is plain string here rather than string | undefined.
test("MyRequired makes everything required and removes the undefined", () => {
  expectTypeOf<MyRequired<Row>>().toEqualTypeOf<{
    readonly id: string;
    label: string;
    count: number;
  }>();
});

// Asserted through the key probes rather than as a whole shape, because
// the natural implementation is an intersection and the flat object it
// describes is not the same TYPE as the intersection.
test("PartialBy makes exactly the named key optional", () => {
  expectTypeOf<OptionalKeys<PartialBy<Row, "count">>>().toEqualTypeOf<"label" | "count">();
  expectTypeOf<RequiredKeys<PartialBy<Row, "count">>>().toEqualTypeOf<"id">();
});

test("PartialBy leaves the value types alone", () => {
  expectTypeOf<PartialBy<Row, "count">["count"]>().toEqualTypeOf<number | undefined>();
  expectTypeOf<PartialBy<Row, "count">["id"]>().toEqualTypeOf<string>();
});

// A fact rejecting PartialBy<Row, "nope"> was written and dropped: the stub
// already declares `K extends keyof T`, so it was green on the untouched
// tree. ex042's MyOmit grades the constraint, from a stub that starts loose.
