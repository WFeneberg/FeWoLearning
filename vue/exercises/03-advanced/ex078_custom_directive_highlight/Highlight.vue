<!-- Exercise 078 — Highlight component (advanced).
     Goal:   implement a `v-highlight` custom directive that sets an
             element's background color from its binding value on
             `mounted`/`updated`, and apply it to a box using a `color` prop.
     Drills: custom directive definition, directive lifecycle hooks
             (mounted/updated), binding.value, local directive registration. -->
<script setup lang="ts">
import type { Directive } from "vue";

defineProps<{ color: string }>();

const vHighlight: Directive<HTMLElement, string> = {
  mounted(_el, _binding) {
    throw new Error("TODO: implement vHighlight mounted hook");
  },
  updated(_el, _binding) {
    throw new Error("TODO: implement vHighlight updated hook");
  },
};
</script>

<template>
  <div data-testid="box" v-highlight="color">Highlighted box</div>
</template>
