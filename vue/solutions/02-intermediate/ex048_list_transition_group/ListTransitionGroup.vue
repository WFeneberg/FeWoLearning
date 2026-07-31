<!-- Exercise 048 — ListTransitionGroup component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

interface ListItem {
  id: number;
  label: string;
}

const items = ref<ListItem[]>([
  { id: 1, label: "Alpha" },
  { id: 2, label: "Bravo" },
  { id: 3, label: "Charlie" },
]);

function removeItem(id: number): void {
  items.value = items.value.filter((item) => item.id !== id);
}
</script>

<template>
  <TransitionGroup name="list" tag="ul" data-testid="list">
    <li v-for="item in items" :key="item.id" data-testid="list-item">
      {{ item.label }}
      <button type="button" :aria-label="`remove-${item.id}`" @click="removeItem(item.id)">
        Remove
      </button>
    </li>
  </TransitionGroup>
</template>

<style scoped>
.list-enter-active,
.list-leave-active {
  transition: all 0.3s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
.list-leave-active {
  position: absolute;
}
</style>
