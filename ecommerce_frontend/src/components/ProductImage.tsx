import type { CSSProperties } from 'react';
import Image from 'react-bootstrap/esm/Image';
import { imagesUrl } from '../api';

type Props = {
  productSrc: string;
  fallback: string;
  className?: string;
  style?: CSSProperties;
  fluid?: boolean;
  thumbnail?: boolean;
};

function ProductImage({ productSrc, fallback, className, style, fluid, thumbnail }: Props) {
  return (
    <Image
      src={`${imagesUrl}/products/${productSrc}`}
      onError={(e) => {
        e.currentTarget.onerror = null;
        e.currentTarget.src = fallback;
      }}
      className={className}
      style={style}
      fluid={fluid}
      thumbnail={thumbnail}
    />
  );
}

export default ProductImage;
