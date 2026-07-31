<!--
  Exercise 053 — ChildCounter component (intermediate).
  Goal:   expose an imperative `reset()` method from a child component so a
          parent can call it via a template ref.
  Drills: internal component state, `defineExpose`, keeping exposed API
          minimal and typed.
-->
<script setup lang="ts">
import { ref } from "vue";

const count = ref(0);

function increment(): void {
  count.value += 1;
}

function reset(): void {
  throw new Error("TODO: implement ChildCounter reset (and expose it)");
}

// TODO: defineExpose the reset() method so a parent template ref can call it.
</script>

<template>
  <div class="child-counter">
    <p class="child-count">Count: {{ count }}</p>
    <button type="button" class="child-increment" @click="increment">
      Increment
    </button>
  </div>
</template>
