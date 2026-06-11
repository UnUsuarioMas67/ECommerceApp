import Button from 'react-bootstrap/Button';
import type { ButtonProps } from 'react-bootstrap/Button';
import { Link } from 'react-router';
import type { LinkProps } from 'react-router';
import type { ReactNode } from 'react';

export type LinkButtonProps = Omit<ButtonProps, 'as' | 'href'> & LinkProps & {
  children?: ReactNode;
};

function LinkButton({ children, ...props }: LinkButtonProps) {
  // @ts-expect-error - conflicto de tipos entre BaseButtonProps.as (keyof IntrinsicElements)
  // y BsPrefixProps.as (React.ElementType). Funciona correctamente en runtime.
  return <Button as={Link} {...props}>{children}</Button>;
}

export default LinkButton;