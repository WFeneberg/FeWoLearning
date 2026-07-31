<!--
  Exercise 059 — DynamicComponentKeepState component (intermediate).
  Goal:   render one of two child components with `<component :is="...">`,
          switching by component identity (not KeepAlive), so each child's
          local state is freshly created every time it mounts.
  Drills: dynamic components, component identity/remount semantics, local
          component state lifecycle, exposing reactive state for testing.
-->
<script setup lang="ts">
import { ref, computed } from "vue";

const PanelA = {
  name: "PanelA",
  props: { start: { type: Number, default: 0 } },
  template: `<section><button @click="count++">{{ count }}</button></section>`,
  data() {
    return { count: this.start };
  },
};

const PanelB = {
  name: "PanelB",
  props: { start: { type: Number, default: 0 } },
  template: `<section><button @click="count++">{{ count }}</button></section>`,
  data() {
    return { count: this.start };
  },
};

export type PanelName = "a" | "b";

const panels: Record<PanelName, unknown> = {
  a: PanelA,
  b: PanelB,
};

const active = ref<PanelName>("a");

const currentComponent = computed(() => {
  throw new Error("TODO: resolve panels[active.value]");
});

function switchTo(_name: PanelName): void {
  throw new Error("TODO: set active.value = name");
}

defineExpose({ active, switchTo });
</script>

<template>
  <div class="dynamic-component-keep-state">
    <component :is="currentComponent" :start="active === 'a' ? 0 : 100" />
  </div>
</template>
