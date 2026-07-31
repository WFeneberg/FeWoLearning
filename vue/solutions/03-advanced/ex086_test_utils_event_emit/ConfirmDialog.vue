<!-- Exercise 086 — ConfirmDialog component (reference solution). -->
<script setup lang="ts">
defineProps<{ message: string }>();
const emit = defineEmits<{ confirm: []; cancel: [] }>();
</script>

<template>
  <div>
    <p>{{ message }}</p>
    <button type="button" @click="emit('confirm')">Confirm</button>
    <button type="button" @click="emit('cancel')">Cancel</button>
  </div>
</template>
