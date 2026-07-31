<!-- Exercise 086 — ConfirmDialog component (advanced).
     Goal:   render a dialog with a message and two buttons ("Confirm" and
             "Cancel") that emit `confirm` / `cancel` events respectively when
             clicked. Neither event carries a payload.
     Drills: @vue/test-utils event emission assertions — wrapper.emitted(),
             asserting event name + call count independently per button. -->
<script setup lang="ts">
defineProps<{ message: string }>();
defineEmits<{ confirm: []; cancel: [] }>();
</script>

<template>
  <div>TODO: render the message and the two buttons</div>
</template>
