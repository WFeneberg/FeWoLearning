<!--
  Exercise 043 — Modal component (intermediate).
  Goal:   render modal content through <Teleport> to a #modal-root target
          element instead of inline in the component's own subtree.
  Drills: <Teleport to="...">, teleport target selectors, v-if with
          Teleport, emitting a close event from teleported content.
-->
<template>
  <!-- TODO: teleport the modal markup to "#modal-root" when `open` is true -->
</template>

<script setup lang="ts">
defineProps<{
  open: boolean;
  title: string;
}>();

defineEmits<{
  (e: "close"): void;
}>();

throw new Error("TODO: implement Modal");
</script>
