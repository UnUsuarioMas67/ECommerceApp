import { useEffect, useState } from 'react';

export function useLocalStorage<T>(key: string, initialVal: T | (() => T)) {
  const [value, setValue] = useState(() => {
    const json = localStorage.getItem(key);
    if (json) {
      return JSON.parse(json) as T;
    }

    if (typeof initialVal === 'function') {
      return (initialVal as () => T)();
    } else {
      return initialVal;
    }
  });

  useEffect(() => {
    localStorage.setItem(key, JSON.stringify(value))
  }, [key, value])

  return [value, setValue] as [typeof value, typeof setValue]
}
