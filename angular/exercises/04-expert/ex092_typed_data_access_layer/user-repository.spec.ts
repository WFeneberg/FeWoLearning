import { UserRepository } from "./user-repository";

describe("UserRepository (typed DTO mapping + error envelope)", () => {
  it("maps a valid active-user DTO to the domain User type", async () => {
    const repo = new UserRepository(async () => ({
      id: "u1",
      full_name: "Ada Lovelace",
      status: "active",
    }));

    const result = await repo.getUser("u1");

    expect(result).toEqual({
      kind: "ok",
      value: { id: "u1", displayName: "Ada Lovelace", isActive: true },
    });
  });

  it("maps status \"inactive\" to isActive: false", async () => {
    const repo = new UserRepository(async () => ({
      id: "u2",
      full_name: "Grace Hopper",
      status: "inactive",
    }));

    const result = await repo.getUser("u2");

    expect(result).toEqual({
      kind: "ok",
      value: { id: "u2", displayName: "Grace Hopper", isActive: false },
    });
  });

  it("returns a typed not-found error when the fetcher resolves null", async () => {
    const repo = new UserRepository(async () => null);

    const result = await repo.getUser("missing");

    expect(result).toEqual({ kind: "error", error: { type: "not-found", id: "missing" } });
  });

  it("returns a typed not-found error when the fetcher resolves undefined", async () => {
    const repo = new UserRepository(async () => undefined);

    const result = await repo.getUser("missing-2");

    expect(result).toEqual({ kind: "error", error: { type: "not-found", id: "missing-2" } });
  });

  it("returns a typed invalid-response error for a payload missing a required field", async () => {
    const repo = new UserRepository(async () => ({ id: "u3", status: "active" }));

    const result = await repo.getUser("u3");

    expect(result).toEqual({
      kind: "error",
      error: { type: "invalid-response", reason: "malformed user payload" },
    });
  });

  it("returns a typed invalid-response error for a payload with the wrong field types", async () => {
    const repo = new UserRepository(async () => ({ id: 42, full_name: "x", status: "active" }));

    const result = await repo.getUser("u4");

    expect(result).toEqual({
      kind: "error",
      error: { type: "invalid-response", reason: "malformed user payload" },
    });
  });

  it("returns a typed invalid-response error for an unrecognized status value", async () => {
    const repo = new UserRepository(async () => ({
      id: "u5",
      full_name: "Unknown Status",
      status: "pending",
    }));

    const result = await repo.getUser("u5");

    expect(result).toEqual({
      kind: "error",
      error: { type: "invalid-response", reason: "unrecognized status: pending" },
    });
  });

  it("returns a typed network error when the fetcher rejects with an Error", async () => {
    const repo = new UserRepository(async () => {
      throw new Error("connection reset");
    });

    const result = await repo.getUser("u6");

    expect(result).toEqual({ kind: "error", error: { type: "network", message: "connection reset" } });
  });

  it("returns a typed network error when the fetcher throws a non-Error value", async () => {
    const repo = new UserRepository(async () => {
      throw "just a string failure";
    });

    const result = await repo.getUser("u7");

    expect(result).toEqual({
      kind: "error",
      error: { type: "network", message: "just a string failure" },
    });
  });

  it("never throws out of getUser, even when the fetcher itself throws synchronously", async () => {
    const repo = new UserRepository(() => {
      throw new Error("synchronous boom");
    });

    await expect(repo.getUser("u8")).resolves.toEqual({
      kind: "error",
      error: { type: "network", message: "synchronous boom" },
    });
  });
});
