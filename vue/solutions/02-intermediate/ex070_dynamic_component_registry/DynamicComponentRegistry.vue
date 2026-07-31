<!--
  Exercise 070 — DynamicComponentRegistry component (reference solution).
-->
<script setup lang="ts">
import { computed, type Component } from "vue";

export interface DynamicComponentRegistryProps {
  registry: Record<string, Component>;
  type: string;
  fallback: Component;
}

const props = defineProps<DynamicComponentRegistryProps>();

const resolved = computed<Component>(() => {
  return props.registry[props.type] ?? props.fallback;
});
</script>

<template>
  <component :is="resolved" />
</template>
