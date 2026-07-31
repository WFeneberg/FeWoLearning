<!--
  Exercise 059 — DynamicComponentKeepState component (reference solution).
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

const currentComponent = computed(() => panels[active.value]);

function switchTo(name: PanelName): void {
  active.value = name;
}

defineExpose({ active, switchTo });
</script>

<template>
  <div class="dynamic-component-keep-state">
    <component :is="currentComponent" :start="active === 'a' ? 0 : 100" />
  </div>
</template>
