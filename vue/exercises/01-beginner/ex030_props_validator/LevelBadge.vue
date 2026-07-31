<!--
  Exercise 030 — LevelBadge component (beginner).
  Goal:   restrict a prop's accepted values with a custom `validator`.
  Drills: prop definitions, validator functions, rendering a prop value.
-->
<script setup lang="ts">
// TODO: define a `level` prop of type String whose validator only allows
// the values "low", "medium", or "high". Until implemented, this throws.
throw new Error("TODO: implement the level prop with a validator");
</script>

<template>
  <span class="level-badge">{{ level }}</span>
</template>
