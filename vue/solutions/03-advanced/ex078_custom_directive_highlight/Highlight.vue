<!-- Exercise 078 — Highlight component (reference solution). -->
<script setup lang="ts">
import type { Directive } from "vue";

defineProps<{ color: string }>();

const vHighlight: Directive<HTMLElement, string> = {
  mounted(el, binding) {
    el.style.backgroundColor = binding.value;
  },
  updated(el, binding) {
    el.style.backgroundColor = binding.value;
  },
};
</script>

<template>
  <div data-testid="box" v-highlight="color">Highlighted box</div>
</template>
