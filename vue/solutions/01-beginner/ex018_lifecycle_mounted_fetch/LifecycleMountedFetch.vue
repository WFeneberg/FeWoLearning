<!--
Exercise 018 — LifecycleMountedFetch component (reference solution).
-->
<script setup lang="ts">
import { ref, onMounted } from "vue";

const loaded = ref(false);

onMounted(() => {
  loaded.value = true;
});

defineExpose({ loaded });
</script>

<template>
  <div>{{ loaded ? "Loaded" : "Loading..." }}</div>
</template>
