# Payment Gateway

## How to Run

1) Build and Run in an IDE of your choice or open a terminal within the main project (payment-gateway/PaymentGateway) & dotnet run
2) Run cli cmd: dotnet test, from the root. Or run via the IDE. Integration Tests should first launch the bank simulator: docker compose from root.

## Design Considerations
For this implementation I’ve adapted a *DDD-lite* approach.
I say lite because the actual domain itself is small.
The core concept is a shallow payment entity and its lifecycle. 
So a heavy DDD setup (factories, repositories everywhere) would have been overkill. 
It's also specified that I should avoid over-engineering.
That said, I still find it valuable to start a new project by identifying the domain concepts, and then let the application and infrastructure layers grow naturally around them from this base.

The result is a layered design, a separation of concerns, and not too much unnecessary abstraction.

### Validating the Payment
Initially I considered using FluentValidation within the controller. I moved away from this idea, as I wanted to minimise dependencies and one of the core tasks of the Payment Gateway is to validate payments.
So this would fall under business logic. Alongside this I intended to keep the controllers thin, acting more like adapters. 
I configured the validation rules themselves as independent, deterministic functions close to the core domain concepts, and orchestrated them within the application layer’s "Payment Validator".
This helped scope unit tests for the individual validation rules easily, with lots of edge case test scenarios.

### Orchestrating the Payment Processing
The payment processing is orchestrated within the application layer, within the "Payment Service". The end2end flow is as follows:
1) The controller receives the request, and maps it to a dto to be passed down to the application layer.
2) The Payment Service receives the dto, and first calls the Payment Validator to validate the payment. If the validation fails, return a validation model with the relevant error messages.
3) If the validation succeeds, the Payment Service then calls the Bank Simulator (via the Bank Client) to process the payment. The bank client is essentially a wrapper for the HttpClient, and is responsible for making the http request and some error handling. 
4) The response from the bank is recorded in the payment entity, and saved into the  (in-memory) database. 
5) The payment entity is mapped to a dto (which masks full card number ect..) and returned to the controller, which then returns it to the client.

### Retrieving a Payment

1) The retrieval of a payment is quite straightforward. The controller receives the request, and passes the id to the application layer. 
2) The Payment Service then retrieves the payment from the in-memory database. I know the documentation specifies that we dont need to create a real database, 
however I created the solution from the ground up (I didnt fork the example project) and felt it was just as quick to make a simple dbcontext with an in-memory provider than to create a custom repository that returns a dummy response.
3) The payment entity is then mapped to a dto, and returned to the controller, which then returns it to the client. 
The mapper takes care of obfuscating the card number, and mapping the expiry date to the correct format.

## Assumptions

### Validation
I've made some assumptions on what is expected at certain stages of the process, when a payment fails validation.
- The documentation doesn't specifiy what the response should look like when a payment fails validation. Only that we have a rejected status. 
I decided to return a model with the relevant error messages, so that the client can understand why the payment was rejected. Along with the "rejected" status.
- It is specified that we dont call the bank if validation fails. I've assumed in this scenario that we also do not record the attempt in the database. It's not specified, but seems logical to me.

### Bank Client
- Handling of the bank simulator is quite vague in the documentation. It isn't specified how to handle scenarios where the bank simulator is down, or returns an error. i.e. 503 
I've taken the liberty of assuming the 'declined' status as the default one, in this scenario. I wanted to avoid throwing unnecessary exceptions 
and to keep expected failure scenarios explicit. I would've like to return a specific response for this scenario, 
however the documentation specifies: "your API design and architecture should be focused on meeting the functional requirements outlined above." So i've avoided introducing additional behaviour beyond this.

## Testing

### Unit Tests

Any and all conditional logic should be covered by unit tests. I've tried to hit as many validation edge case scenarios as I could think of.
I've skipped controller level unit tests as all we would be testing he are mocks, and the code coverage should be handled by our integration level tests.

I've also implemented an ITimeProvider, which can be mocked. This allows the time based functions to be tested in a consistent way.

### Integration Tests

The integration tests cover the end2end flow of the payment processing. I've created one for each realistic response a merchant can expect. 
One thing worth noting is I wouldn't ordinarily create integration tests which rely on a third party (the bank simulator in this case), being a dependency. I prefer the test suite to be self contained.
Ordinarily I would create a conditional stub than can be configured to sit in its place when offline.
The simulator is a special case as it is essentially a stub itself, and is designed to be run locally. And I wanted to avoid doing to many things not specified the requirements.

## Reflection/ What I would do differently next time / Suggestions for improvement
- With regard to the validation, I created the validation rules as static functions for swiftness and ease of use. What would have been nice, would be to create (non-static) with a common interface for them.
This would have allowed me to mock them in the Validator Unit test. Due to this lack of isolation, the unit tests for the validator are more like integration tests, as they are testing the actual validation rules, rather than just the orchestration of them. 
A common interface would've allowed me to do something fancy like use the strategy pattern and preload the rules into a list that can be iterated over. Ultimately I went for simplicity
as I want to avoid over engineering, and the current implementation is straightforward and easy to understand.

- Another area I would improve is the error handling for the bank client. It mostly functions as a wrapper for the HttpClient, but I would like to implement some retry logic for transient errors
and also some better handling for different response codes. For example, if the bank simulator is down, it would be nice to have a specific response for this, rather than just a generic "declined" status.

- Maybe some sort of idempotency key for the payment processing. Did this exact payment and card just get submitted? 
