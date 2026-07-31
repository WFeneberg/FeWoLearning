<!--
  Exercise 052 — TemplateRefFocus component (intermediate).
  Goal:   use a template ref to access a real DOM element and imperatively
          focus it once the component has mounted.
  Drills: `ref` template refs (`useTemplateRef`/`ref<HTMLElement>`), onMounted,
          calling native DOM methods from Vue.
-->
<script setup lang="ts">
import { onMounted, ref } from "vue";

const inputRef = ref<HTMLInputElement | null>(null);

onMounted(() => {
  throw new Error("TODO: implement TemplateRefFocus (focus the input on mount)");
});
</script>

<template>
  <input ref="inputRef" type="text" aria-label="focus-me" />
</template>
