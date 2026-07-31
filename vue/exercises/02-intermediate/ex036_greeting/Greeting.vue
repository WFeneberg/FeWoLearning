<!-- Exercise 036 — Greeting component (intermediate).
     Goal:   render "Hello, {name}!" from a required `name` prop, and emit a
             `greet` event with the name when the button is clicked.
     Drills: typed props (defineProps), events (defineEmits), template binding. -->
<script setup lang="ts">
defineProps<{ name: string }>();
defineEmits<{ greet: [name: string] }>();
</script>

<template>
  <div>TODO: render the greeting and a button</div>
</template>
