<!-- Exercise 016 — PropsTypedMessage component (beginner). -->
<!-- Goal:   declare a typed prop and render it in the template. -->
<!-- Drills: defineProps with TypeScript generic syntax, prop interpolation. -->
<script setup lang="ts">
interface Props {
  message: string;
}

defineProps<Props>();

throw new Error("TODO: implement PropsTypedMessage");
</script>

<template>
  <p class="message">{{ message }}</p>
</template>
