type Props = {
  text?: string;
};

function Title({ text }: Props) {
  const title = text ? `${text} - PC Tech Solutions` : 'PC Tech Solutions';

  return <title>{title}</title>;
}

export default Title;