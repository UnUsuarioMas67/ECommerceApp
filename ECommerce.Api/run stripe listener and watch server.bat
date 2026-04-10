stripe login

START stripe listen --forward-to localhost:5113/api/checkout/webhook --events checkout.session.completed,checkout.session.expired,payment_intent.succeeded

START dotnet watch