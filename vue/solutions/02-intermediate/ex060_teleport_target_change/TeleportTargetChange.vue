<!--
  Exercise 060 — Teleport with dynamic target/disabled (reference solution).
-->
<script lang="ts">
export const TELEPORT_TARGET = "#teleport-target-060";
</script>

<script setup lang="ts">
import { ref, computed } from "vue";

function useOverlay(initial = false) {
  const isOpen = ref(initial);
  const toggle = () => {
    isOpen.value = !isOpen.value;
  };
  return { isOpen, toggle };
}

const { isOpen, toggle } = useOverlay();

// Teleport only while the overlay is open; otherwise keep it in place.
const disabled = computed<boolean>(() => !isOpen.value);
</script>

<template>
  <div class="host">
    <button type="button" data-testid="toggle" @click="toggle">
      {{ isOpen ? "Close" : "Open" }}
    </button>
    <Teleport :to="TELEPORT_TARGET" :disabled="disabled">
      <div data-testid="content" class="panel">Overlay content</div>
    </Teleport>
  </div>
</template>
