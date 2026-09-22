import { describe, expect, it } from "vitest";
import { dataOnly } from "@ex/02-intermediate/ex038_key_remapping/index";
import type { Api } from "@ex/02-intermediate/ex038_key_remapping/index";

function makeApi(): Api {
  return {
    getUser: () => "ada",
    getPost: () => "hello",
    id: 7,
    label: "main",
  };
}

describe("ex038 dataOnly", () => {
  it("keeps the data properties", () => {
    expect(dataOnly(makeApi())).toEqual({ id: 7, label: "main" });
  });

  it("drops the methods", () => {
    // The cast is needed only because DataOf is still `unknown` on the
    // untouched stub, and Object.keys does not take an unknown.
    const data = dataOnly(makeApi()) as object;
    expect(Object.keys(data).sort()).toEqual(["id", "label"]);
  });
});
