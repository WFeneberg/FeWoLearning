<!-- Exercise 080 — KeepAliveCache component (reference solution). -->
<script setup lang="ts">
import { defineComponent, shallowRef } from "vue";

const NotesTab = defineComponent({
  name: "NotesTab",
  setup() {
    const text = shallowRef("");
    return { text };
  },
  template: `<textarea data-testid="notes-input" v-model="text" />`,
});

const InfoTab = defineComponent({
  name: "InfoTab",
  template: `<p data-testid="info-panel">Info panel — nothing to persist here.</p>`,
});

const tabs = {
  notes: NotesTab,
  info: InfoTab,
} as const;

type TabKey = keyof typeof tabs;

const activeTab = shallowRef<TabKey>("notes");

function selectTab(key: TabKey): void {
  activeTab.value = key;
}
</script>

<template>
  <div>
    <button type="button" data-testid="tab-btn-notes" @click="selectTab('notes')">
      Notes
    </button>
    <button type="button" data-testid="tab-btn-info" @click="selectTab('info')">
      Info
    </button>

    <KeepAlive>
      <component :is="tabs[activeTab]" />
    </KeepAlive>
  </div>
</template>
