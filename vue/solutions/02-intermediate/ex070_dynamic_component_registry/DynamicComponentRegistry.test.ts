import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import DynamicComponentRegistry from "./DynamicComponentRegistry.vue";

const TextPanel = {
  name: "TextPanel",
  template: `<article class="text-panel">Text content</article>`,
};

const ImagePanel = {
  name: "ImagePanel",
  template: `<figure class="image-panel">Image content</figure>`,
};

const VideoPanel = {
  name: "VideoPanel",
  template: `<div class="video-panel">Video content</div>`,
};

const UnknownTypePanel = {
  name: "UnknownTypePanel",
  template: `<div class="unknown-panel">Unsupported content</div>`,
};

const registry = {
  text: TextPanel,
  image: ImagePanel,
  video: VideoPanel,
};

describe("DynamicComponentRegistry", () => {
  it("renders the component mapped to the 'text' type", () => {
    const wrapper = mount(DynamicComponentRegistry, {
      props: { registry, type: "text", fallback: UnknownTypePanel },
    });
    expect(wrapper.find(".text-panel").exists()).toBe(true);
    expect(wrapper.text()).toBe("Text content");
  });

  it("renders the component mapped to the 'image' type", () => {
    const wrapper = mount(DynamicComponentRegistry, {
      props: { registry, type: "image", fallback: UnknownTypePanel },
    });
    expect(wrapper.find(".image-panel").exists()).toBe(true);
    expect(wrapper.text()).toBe("Image content");
  });

  it("renders the component mapped to the 'video' type", () => {
    const wrapper = mount(DynamicComponentRegistry, {
      props: { registry, type: "video", fallback: UnknownTypePanel },
    });
    expect(wrapper.find(".video-panel").exists()).toBe(true);
    expect(wrapper.text()).toBe("Video content");
  });

  it("renders the fallback component for an unknown type", () => {
    const wrapper = mount(DynamicComponentRegistry, {
      props: { registry, type: "audio", fallback: UnknownTypePanel },
    });
    expect(wrapper.find(".unknown-panel").exists()).toBe(true);
    expect(wrapper.text()).toBe("Unsupported content");
  });

  it("re-resolves reactively when the type prop changes", async () => {
    const wrapper = mount(DynamicComponentRegistry, {
      props: { registry, type: "text", fallback: UnknownTypePanel },
    });
    expect(wrapper.text()).toBe("Text content");

    await wrapper.setProps({ type: "video" });
    expect(wrapper.text()).toBe("Video content");

    await wrapper.setProps({ type: "unknown" });
    expect(wrapper.text()).toBe("Unsupported content");
  });
});
