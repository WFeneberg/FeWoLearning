export interface Request {
  url: string;
  headers: Record<string, string>;
}

export function makeRequest(url: string): Request {
  return { url, headers: {} };
}

export const VERSION = "1.0.0";
