// Exercise 020 — useSettingsWatcher composable (reference solution).
import { reactive, ref, watch, type Ref } from "vue";

export interface Settings {
  theme: string;
  notifications: {
    email: boolean;
    sms: boolean;
  };
}

export interface SettingsWatcher {
  settings: Settings;
  changeCount: Ref<number>;
}

export function useSettingsWatcher(): SettingsWatcher {
  const settings = reactive<Settings>({
    theme: "light",
    notifications: {
      email: true,
      sms: true,
    },
  });
  const changeCount = ref(0);

  watch(
    settings,
    () => {
      changeCount.value += 1;
    },
    { deep: true },
  );

  return { settings, changeCount };
}
