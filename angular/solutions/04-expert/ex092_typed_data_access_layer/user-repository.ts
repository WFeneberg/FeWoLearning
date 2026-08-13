// Exercise 092 — a typed data access layer: DTO mapping and a typed error envelope (reference solution).

export interface User {
  readonly id: string;
  readonly displayName: string;
  readonly isActive: boolean;
}

export type Result<T, E> =
  | { readonly kind: "ok"; readonly value: T }
  | { readonly kind: "error"; readonly error: E };

export type UserRepositoryError =
  | { readonly type: "not-found"; readonly id: string }
  | { readonly type: "invalid-response"; readonly reason: string }
  | { readonly type: "network"; readonly message: string };

interface RawUserDto {
  readonly id: string;
  readonly full_name: string;
  readonly status: string;
}

export type UserFetcher = (id: string) => Promise<unknown>;

function isRawUserDto(value: unknown): value is RawUserDto {
  if (typeof value !== "object" || value === null) {
    return false;
  }
  const candidate = value as Record<string, unknown>;
  return (
    typeof candidate["id"] === "string" &&
    typeof candidate["full_name"] === "string" &&
    typeof candidate["status"] === "string"
  );
}

export class UserRepository {
  constructor(private readonly fetchUser: UserFetcher) {}

  async getUser(id: string): Promise<Result<User, UserRepositoryError>> {
    let raw: unknown;
    try {
      raw = await this.fetchUser(id);
    } catch (err) {
      return {
        kind: "error",
        error: { type: "network", message: err instanceof Error ? err.message : String(err) },
      };
    }

    if (raw === null || raw === undefined) {
      return { kind: "error", error: { type: "not-found", id } };
    }

    if (!isRawUserDto(raw)) {
      return { kind: "error", error: { type: "invalid-response", reason: "malformed user payload" } };
    }

    if (raw.status !== "active" && raw.status !== "inactive") {
      return {
        kind: "error",
        error: { type: "invalid-response", reason: `unrecognized status: ${raw.status}` },
      };
    }

    return {
      kind: "ok",
      value: { id: raw.id, displayName: raw.full_name, isActive: raw.status === "active" },
    };
  }
}
