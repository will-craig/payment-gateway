namespace PaymentGateway.Configuration;

public static class MiddlewareConfigurator
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Payment Gateway API";
            options.DisplayRequestDuration();
        });
        app.MapControllers();
        return app;
    }
}