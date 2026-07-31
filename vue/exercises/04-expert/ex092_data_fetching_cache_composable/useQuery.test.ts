import { describe, expect, it, vi } from "vitest";
import { defineComponent, h } from "vue";
import { mount, flushPromises } from "@vue/test-utils";
import { useQuery } from "./useQuery";

function makeComponent(key: string, fetcher: () => Promise<string>) {
  return defineComponent({
    setup() {
      const { data, isLoading } = useQuery(key, fetcher);
      return () => h("div", isLoading.value ? "loading" : (data.value ?? ""));
    },
  });
}

describe("useQuery", () => {
  it("fetches and exposes the resolved data", async () => {
    const fetcher = vi.fn(async () => "result-a");
    const { data, isLoading } = useQuery("key-a", fetcher);

    expect(isLoading.value).toBe(true);
    await flushPromises();

    expect(isLoading.value).toBe(false);
    expect(data.value).toBe("result-a");
    expect(fetcher).toHaveBeenCalledTimes(1);
  });

  it("returns cached data synchronously on a repeat call, without re-fetching", async () => {
    const fetcher = vi.fn(async () => "result-b");
    const first = useQuery("key-b", fetcher);
    await flushPromises();
    expect(first.data.value).toBe("result-b");

    const second = useQuery("key-b", fetcher);

    // No loading flicker: the cached value is available immediately.
    expect(second.isLoading.value).toBe(false);
    expect(second.data.value).toBe("result-b");
    expect(fetcher).toHaveBeenCalledTimes(1);
  });

  it("calls the fetcher only once across two components sharing a key", async () => {
    const fetcher = vi.fn(async () => "shared-value");
    const CompA = makeComponent("key-shared", fetcher);
    const CompB = makeComponent("key-shared", fetcher);

    const wrapperA = mount(CompA);
    const wrapperB = mount(CompB);

    expect(wrapperA.text()).toBe("loading");
    expect(wrapperB.text()).toBe("loading");

    await flushPromises();

    expect(wrapperA.text()).toBe("shared-value");
    expect(wrapperB.text()).toBe("shared-value");
    expect(fetcher).toHaveBeenCalledTimes(1);
  });

  it("keeps separate cache entries per key", async () => {
    const fetcherX = vi.fn(async () => "value-x");
    const fetcherY = vi.fn(async () => "value-y");

    const x = useQuery("key-x", fetcherX);
    const y = useQuery("key-y", fetcherY);
    await flushPromises();

    expect(x.data.value).toBe("value-x");
    expect(y.data.value).toBe("value-y");
    expect(fetcherX).toHaveBeenCalledTimes(1);
    expect(fetcherY).toHaveBeenCalledTimes(1);
  });

  it("refetch bypasses the cache and re-invokes the fetcher", async () => {
    const fetcher = vi.fn(async () => "value-1");
    const { data, refetch } = useQuery("key-refetch", fetcher);
    await flushPromises();
    expect(data.value).toBe("value-1");

    fetcher.mockImplementation(async () => "value-2");
    await refetch();

    expect(data.value).toBe("value-2");
    expect(fetcher).toHaveBeenCalledTimes(2);
  });

  it("surfaces a rejected fetch as an error without throwing", async () => {
    const failure = new Error("network down");
    const fetcher = vi.fn(async () => {
      throw failure;
    });
    const { data, error, isLoading } = useQuery("key-error", fetcher);

    await flushPromises();

    expect(isLoading.value).toBe(false);
    expect(data.value).toBeUndefined();
    expect(error.value).toBe(failure);
  });
});
