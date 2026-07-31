<!-- Exercise 016 — PropsTypedMessage component (reference solution). -->
<script setup lang="ts">
interface Props {
  message: string;
}

defineProps<Props>();
</script>

<template>
  <p class="message">{{ message }}</p>
</template>
