<!--
  Exercise 030 — LevelBadge component (reference solution).
-->
<script setup lang="ts">
defineProps({
  level: {
    type: String,
    required: true,
    validator: (value: string) => ["low", "medium", "high"].includes(value),
  },
});
</script>

<template>
  <span class="level-badge">{{ level }}</span>
</template>
