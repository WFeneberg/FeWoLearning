import { describe, expect, it } from "vitest";
import { traceOf, tracedRequest } from "@ex/03-advanced/ex081_module_augmentation/index";
import { makeRequest } from "@ex/03-advanced/ex081_module_augmentation/library";

describe("ex081 tracedRequest", () => {
  it("keeps what the library already produced", () => {
    const request = tracedRequest("/a", "t-1", 1000);
    expect(request.url).toBe("/a");
    expect(request.headers).toEqual({});
  });

  // Augmentation DECLARES the members; the code still has to put them
  // there, and nothing checks that it did.
  it("sets the members the augmentation declared", () => {
    const request = tracedRequest("/a", "t-1", 1000) as unknown as Record<string, unknown>;
    expect(request["traceId"]).toBe("t-1");
    expect(request["startedAt"]).toBe(1000);
  });
});

describe("ex081 traceOf", () => {
  it("reads the trace id when there is one", () => {
    expect(traceOf(tracedRequest("/a", "t-9", 0))).toBe("t-9");
  });

  it("falls back for a request the library made on its own", () => {
    expect(traceOf(makeRequest("/plain"))).toBe("untraced");
  });
});
