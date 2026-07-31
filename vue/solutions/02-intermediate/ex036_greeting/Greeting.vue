<!-- Exercise 036 — Greeting component (reference solution). -->
<script setup lang="ts">
const props = defineProps<{ name: string }>();
const emit = defineEmits<{ greet: [name: string] }>();
</script>

<template>
  <div>
    <p>Hello, {{ props.name }}!</p>
    <button type="button" @click="emit('greet', props.name)">Greet</button>
  </div>
</template>
