<!--
  Exercise 056 — ThemeLabel component (intermediate).
  Goal:   inject a "theme" value provided by an ancestor, falling back to a
          default when no ancestor provides it.
  Drills: inject() with a default value, provide/inject across component
          boundaries, injection keys.
-->
<script setup lang="ts">
import { inject } from "vue";
import { themeKey } from "./themeKey";

// TODO: inject `themeKey`, falling back to "light" when unprovided.
const theme = (() => {
  throw new Error("TODO: implement inject with default");
})();

defineExpose({ theme });
</script>

<template>
  <span class="theme-label">{{ theme }}</span>
</template>
