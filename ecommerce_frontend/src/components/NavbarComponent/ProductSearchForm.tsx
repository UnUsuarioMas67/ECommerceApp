import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import InputGroup from 'react-bootstrap/InputGroup';
import { Search } from 'react-bootstrap-icons';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { searchSchema, type ProductSearch } from '../../schemas';
import { zodResolver } from '@hookform/resolvers/zod';
import { useLocation, useNavigate } from '@tanstack/react-router';
import type { Category } from '../../api/types';

type Props = {
  categories: Category[];
};

function ProductSearchForm({ categories }: Props) {
  const { register, handleSubmit } = useForm<ProductSearch>({ resolver: zodResolver(searchSchema) });
  const { searchTerm, category } = useLocation({select: (state) => state.search});
  const navigate = useNavigate();

  const onSubmit: SubmitHandler<ProductSearch> = (data) => {
    let { category, searchTerm } = data;
    if (!categories.find((c) => c.slug === category)) category = undefined;
    if (searchTerm === '') searchTerm = undefined;

    navigate({ to: '/', search: { searchTerm, category }, reloadDocument: true });
  };

  return (
    <Form
      style={{ maxWidth: '600px' }}
      className="w-100 mx-lg-4 mx-xl-auto mt-2 mt-lg-0"
      data-bs-theme="light"
      onSubmit={handleSubmit(onSubmit)}>
      <Row className="gx-1 gy-2 ">
        <Col lg={2}>
          <Form.Select size="sm" aria-label="Filter by category" {...register('category', { value: category })}>
            <option>-- All --</option>
            {categories.map((category) => (
              <option key={category.id} value={category.slug}>
                {category.name}
              </option>
            ))}
          </Form.Select>
        </Col>

        <Col>
          <InputGroup size="sm">
            <Form.Control
              placeholder="Search..."
              aria-label="Search products"
              aria-describedby="basic-addon2"
              {...register('searchTerm', { value: searchTerm })}
            />

            <Button variant="secondary" id="search-product-btn" type="submit">
              Search <Search />
            </Button>
          </InputGroup>
        </Col>
      </Row>
    </Form>
  );
}

export default ProductSearchForm;
