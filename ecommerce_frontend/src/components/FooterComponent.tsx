import Logo from './Logo';

function FooterComponent({ small }: { small?: boolean }) {
  return (
    <footer className={`bg-light ${small ? 'py-4' : 'py-5'} text-center border-top text-body-secondary`}>
      <Logo size='sm' /><br />
      Copyright @ UnUsuarioMas67
    </footer>
  );
}

export default FooterComponent;
