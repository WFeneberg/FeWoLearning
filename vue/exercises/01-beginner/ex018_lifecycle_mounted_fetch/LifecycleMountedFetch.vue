<!--
Exercise 018 — LifecycleMountedFetch component (beginner).
Goal:   set a `loaded` ref to true inside the onMounted lifecycle hook.
Drills: onMounted, ref, component lifecycle timing.
-->
<script setup lang="ts">
import { ref, onMounted } from "vue";

const loaded = ref(false);

// TODO: use onMounted to set loaded.value to true once the component mounts.
throw new Error("TODO: implement onMounted lifecycle hook");

defineExpose({ loaded });
</script>

<template>
  <div>{{ loaded ? "Loaded" : "Loading..." }}</div>
</template>
