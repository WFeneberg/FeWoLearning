<!-- Exercise 048 — ListTransitionGroup component (intermediate). -->
<!-- Goal:   animate list add/remove using <TransitionGroup>. -->
<!-- Drills: TransitionGroup, keyed v-for, mutating a reactive array from an event handler. -->
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

function removeItem(_id: number): void {
  throw new Error("TODO: implement removeItem (remove the item with matching id from items)");
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
