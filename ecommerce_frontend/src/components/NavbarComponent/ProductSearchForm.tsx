import Form from 'react-bootstrap/Form';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from 'react-bootstrap/Button';
import InputGroup from 'react-bootstrap/InputGroup';
import { Search } from 'react-bootstrap-icons';
import { useAxios } from '../../hooks/use-axios';
import { useSuspenseQuery } from '@tanstack/react-query';
import { getCategories } from '../../api';

function ProductSearchForm() {
  const axiosInstance = useAxios();
  const { data: categories } = useSuspenseQuery({
    queryKey: ['categories'],
    queryFn: async () => await getCategories(axiosInstance),
  });

  return (
    <Form style={{ maxWidth: '900px' }} className="w-100 mx-lg-4 mx-xl-auto mt-2 mt-lg-0" data-bs-theme="light">
      <Row className="gx-1 gy-2">
        <Col lg={2}>
          <Form.Select size="sm" aria-label="Filter by category">
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
            <Form.Control placeholder="Search..." aria-label="Search products" aria-describedby="basic-addon2" />

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
