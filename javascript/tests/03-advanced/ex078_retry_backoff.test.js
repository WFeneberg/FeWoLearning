import { describe, expect, it, vi } from "vitest";
import { retry } from "@ex/03-advanced/ex078_retry_backoff/index.js";

/** Records the delays it was asked for, and never actually waits. */
function recordingSleep() {
  const delays = [];
  return { delays, sleep: async (ms) => void delays.push(ms) };
}

const failing = (failures, value = "ok") => {
  let calls = 0;
  return vi.fn(async () => {
    calls += 1;
    if (calls <= failures) throw new Error(`failure ${calls}`);
    return value;
  });
};

describe("ex078 retry", () => {
  it("returns the first success without sleeping", async () => {
    const { delays, sleep } = recordingSleep();
    const fn = failing(0);
    await expect(retry(fn, { sleep })).resolves.toBe("ok");
    expect(fn).toHaveBeenCalledTimes(1);
    expect(delays).toEqual([]);
  });

  it("retries until it succeeds", async () => {
    const { delays, sleep } = recordingSleep();
    const fn = failing(2);
    await expect(retry(fn, { sleep })).resolves.toBe("ok");
    expect(fn).toHaveBeenCalledTimes(3);
    expect(delays).toEqual([10, 20]);
  });

  it("backs off exponentially from the options", async () => {
    const { delays, sleep } = recordingSleep();
    await expect(
      retry(failing(10), { attempts: 4, baseDelay: 100, factor: 3, sleep }),
    ).rejects.toThrow();
    expect(delays).toEqual([100, 300, 900]);
  });

  it("does not sleep after the final attempt", async () => {
    const { delays, sleep } = recordingSleep();
    await expect(retry(failing(10), { attempts: 2, sleep })).rejects.toThrow();
    expect(delays).toHaveLength(1);
  });

  it("rejects with the LAST error", async () => {
    const { sleep } = recordingSleep();
    await expect(retry(failing(10), { attempts: 3, sleep })).rejects.toThrow("failure 3");
  });

  it("passes the attempt number in, starting at 1", async () => {
    const { sleep } = recordingSleep();
    const seen = [];
    await retry(
      async (attempt) => {
        seen.push(attempt);
        if (attempt < 3) throw new Error("again");
        return "ok";
      },
      { sleep },
    );
    expect(seen).toEqual([1, 2, 3]);
  });

  it("gives up immediately when shouldRetry says no", async () => {
    const { delays, sleep } = recordingSleep();
    const fn = failing(10);
    await expect(retry(fn, { attempts: 5, sleep, shouldRetry: () => false })).rejects.toThrow(
      "failure 1",
    );
    expect(fn).toHaveBeenCalledTimes(1);
    expect(delays).toEqual([]);
  });

  it("gives shouldRetry the error and the attempt number", async () => {
    const { sleep } = recordingSleep();
    const shouldRetry = vi.fn(() => true);
    await expect(retry(failing(10), { attempts: 2, sleep, shouldRetry })).rejects.toThrow();
    expect(shouldRetry).toHaveBeenCalledTimes(1);
    expect(shouldRetry.mock.calls[0][0].message).toBe("failure 1");
    expect(shouldRetry.mock.calls[0][1]).toBe(1);
  });

  it("makes exactly one call for attempts: 1", async () => {
    const { delays, sleep } = recordingSleep();
    const fn = failing(10);
    await expect(retry(fn, { attempts: 1, sleep })).rejects.toThrow("failure 1");
    expect(fn).toHaveBeenCalledTimes(1);
    expect(delays).toEqual([]);
  });
});
