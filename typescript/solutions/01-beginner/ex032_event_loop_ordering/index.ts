// Reference solution — exercise 032.
export function scheduleAll(): Promise<string[]> {
  const log: string[] = [];
  return new Promise((resolve) => {
    log.push("sync");
    queueMicrotask(() => log.push("micro"));
    void Promise.resolve().then(() => log.push("promise"));
    setTimeout(() => {
      log.push("macro");
      // Both microtasks have long since drained by the time any macrotask
      // runs, so the log is complete here.
      resolve(log);
    }, 0);
  });
}

export async function awaitAlwaysYields(): Promise<string[]> {
  const log: string[] = [];

  const inner = async (): Promise<void> => {
    log.push("a");
    // A plain value, not a promise — and it still suspends. The rest of this
    // function is queued as a microtask.
    await 1;
    log.push("b");
  };

  const pending = inner();
  log.push("c");
  await pending;
  return log;
}

export function timerRanDuringBusyWait(ms: number): boolean {
  let fired = false;
  setTimeout(() => {
    fired = true;
  }, 0);

  const deadline = Date.now() + ms;
  while (Date.now() < deadline) {
    // Spin. There is one thread, so the callback above cannot run here —
    // however long this takes.
  }
  return fired;
}
