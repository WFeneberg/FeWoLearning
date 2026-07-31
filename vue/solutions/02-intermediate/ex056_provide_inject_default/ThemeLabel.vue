<!--
  Exercise 056 — ThemeLabel component (reference solution).
-->
<script setup lang="ts">
import { inject } from "vue";
import { themeKey } from "./themeKey";

const theme = inject(themeKey, "light");

defineExpose({ theme });
</script>

<template>
  <span class="theme-label">{{ theme }}</span>
</template>
