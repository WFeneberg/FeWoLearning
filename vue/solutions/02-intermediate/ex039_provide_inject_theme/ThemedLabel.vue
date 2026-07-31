<!--
Exercise 039 — ThemedLabel component (reference solution).
`inject`s the `theme` string provided by an ancestor and renders a themed label.
-->
<template>
  <span class="themed-label" :class="`theme-${theme}`">Theme: {{ theme }}</span>
</template>

<script setup lang="ts">
import { inject } from "vue";
import { themeKey } from "./themeKey";

const theme = inject(themeKey, "none");
</script>
