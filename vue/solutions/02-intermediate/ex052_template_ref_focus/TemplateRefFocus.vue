<!--
  Exercise 052 — TemplateRefFocus component (reference solution).
-->
<script setup lang="ts">
import { onMounted, ref } from "vue";

const inputRef = ref<HTMLInputElement | null>(null);

onMounted(() => {
  inputRef.value?.focus();
});
</script>

<template>
  <input ref="inputRef" type="text" aria-label="focus-me" />
</template>
