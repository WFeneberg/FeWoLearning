<!--
Exercise 039 — ThemeProvider component (intermediate).
Goal:   `provide` a `theme` string to descendant components without prop drilling.
Drills: provide() with an injection key, wiring a prop into a provided value.
-->
<template>
  <div class="theme-provider">
    <!-- TODO: render the default slot so descendants (e.g. ThemedLabel) can inject the theme -->
  </div>
</template>

<script setup lang="ts">
import { themeKey } from "./themeKey";

defineProps<{ theme: string }>();

throw new Error("TODO: implement ThemeProvider");
</script>
