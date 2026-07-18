import { PcDisplay } from 'react-bootstrap-icons';

type Props = {
  size?: 'sm' | 'lg';
};

function Logo({ size }: Props) {
  const fontSize = size === 'sm' ? 'fs-6' : size === 'lg' ? 'fs-3' : 'fs-5';

  return (
    <span className={`d-inline-flex align-items-center gap-1 fw-bold ${fontSize}`}>
      <PcDisplay /> PC Tech Solutions
    </span>
  );
}

export default Logo;
