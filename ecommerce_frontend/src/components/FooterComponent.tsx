function FooterComponent({ small }: { small?: boolean}) {
  return (
    <footer className={`bg-light ${small ? 'py-4' : 'py-5'} text-center border-top text-body-secondary`}>Copyright @ UnUsuarioMas67</footer>
  );
}

export default FooterComponent;
