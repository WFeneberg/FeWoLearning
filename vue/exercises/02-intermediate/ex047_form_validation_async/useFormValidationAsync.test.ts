import { describe, expect, it, vi } from "vitest";
import { useFormValidationAsync } from "./useFormValidationAsync";

describe("useFormValidationAsync", () => {
  it("has no error before validation runs", () => {
    const checkAvailable = vi.fn().mockResolvedValue(true);
    const { error, isValidating } = useFormValidationAsync(checkAvailable);
    expect(error.value).toBeNull();
    expect(isValidating.value).toBe(false);
  });

  it("sets an error when checkAvailable resolves false", async () => {
    const checkAvailable = vi.fn().mockResolvedValue(false);
    const { username, error, validate } = useFormValidationAsync(checkAvailable);
    username.value = "taken-name";

    const result = await validate();

    expect(checkAvailable).toHaveBeenCalledWith("taken-name");
    expect(result).toBe(false);
    expect(error.value).toBe("Username is already taken");
  });

  it("clears any prior error when checkAvailable resolves true", async () => {
    const checkAvailable = vi
      .fn()
      .mockResolvedValueOnce(false)
      .mockResolvedValueOnce(true);
    const { username, error, validate } = useFormValidationAsync(checkAvailable);
    username.value = "taken-name";
    await validate();
    expect(error.value).toBe("Username is already taken");

    username.value = "free-name";
    const result = await validate();

    expect(result).toBe(true);
    expect(error.value).toBeNull();
  });

  it("tracks isValidating while the async check is in flight", async () => {
    let resolveCheck!: (value: boolean) => void;
    const checkAvailable = vi.fn(
      () =>
        new Promise<boolean>((resolve) => {
          resolveCheck = resolve;
        }),
    );
    const { username, isValidating, validate } = useFormValidationAsync(checkAvailable);
    username.value = "someone";

    const pending = validate();
    expect(isValidating.value).toBe(true);

    resolveCheck(true);
    await pending;

    expect(isValidating.value).toBe(false);
  });

  it("rejects an empty username without calling checkAvailable", async () => {
    const checkAvailable = vi.fn().mockResolvedValue(true);
    const { username, error, validate } = useFormValidationAsync(checkAvailable);
    username.value = "";

    const result = await validate();

    expect(result).toBe(false);
    expect(error.value).toBe("Username is required");
    expect(checkAvailable).not.toHaveBeenCalled();
  });
});
