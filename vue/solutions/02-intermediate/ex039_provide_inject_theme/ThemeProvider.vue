<!--
Exercise 039 — ThemeProvider component (reference solution).
`provide`s a `theme` string to descendant components without prop drilling.
-->
<template>
  <div class="theme-provider">
    <slot />
  </div>
</template>

<script setup lang="ts">
import { provide } from "vue";
import { themeKey } from "./themeKey";

const props = defineProps<{ theme: string }>();

provide(themeKey, props.theme);
</script>
