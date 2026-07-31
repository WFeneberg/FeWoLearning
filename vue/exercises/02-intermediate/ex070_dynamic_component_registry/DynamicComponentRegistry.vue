<!--
  Exercise 070 — DynamicComponentRegistry component (intermediate).
  Goal:   resolve `<component :is="...">` from a lookup object of
          type-to-component mappings, falling back for unknown types.
  Drills: dynamic components, computed resolution from props, fallback
          rendering when a key is missing from a registry.
-->
<script setup lang="ts">
import { computed, type Component } from "vue";

export interface DynamicComponentRegistryProps {
  registry: Record<string, Component>;
  type: string;
  fallback: Component;
}

const props = defineProps<DynamicComponentRegistryProps>();

// TODO: resolve to props.registry[props.type] when that key exists on the
// registry, otherwise fall back to props.fallback.
const resolved = computed<Component>(() => {
  throw new Error("TODO: implement DynamicComponentRegistry resolution");
});
</script>

<template>
  <component :is="resolved" />
</template>
