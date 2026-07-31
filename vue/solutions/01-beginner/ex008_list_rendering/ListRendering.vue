<!--
Exercise 008 — ListRendering component (reference solution).
-->
<script setup lang="ts">
defineProps<{
  items: string[];
}>();
</script>

<template>
  <ul>
    <li v-for="(item, index) in items" :key="index">{{ item }}</li>
  </ul>
</template>
