export function lengthError(prop: string, amount: number, type: 'min' | 'max') {
  const w = type === 'min' ? 'fewer' : 'more';
  return `${prop} cannot contain ${w} than ${amount} characters`;
}

export function required(prop: string) {
  return `${prop} is required`;
}