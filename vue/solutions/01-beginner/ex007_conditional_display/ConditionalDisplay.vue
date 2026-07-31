<!--
  Exercise 007 — ConditionalDisplay component (reference solution).
-->
<script setup lang="ts">
defineProps<{
  status: "loading" | "error" | "ready";
}>();
</script>

<template>
  <p v-if="status === 'loading'">Loading</p>
  <p v-else-if="status === 'error'">Error</p>
  <p v-else>Ready</p>
</template>
