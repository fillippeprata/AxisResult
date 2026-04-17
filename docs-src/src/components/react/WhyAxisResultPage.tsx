import React from 'react';
import { Layout } from './shared/Layout';
import { CodeBlock } from './shared/CodeBlock';

export const WhyAxisResultPage: React.FC = () => {
    const badCode = `public async Task<ApiResponse> Handle(CreateOrderCommand cmd)
{
    try
    {
        var customer = await _customerRepo.GetByIdAsync(cmd.CustomerId);
        if (customer == null)
            throw new NotFoundException("Customer not found");

        if (!customer.IsActive)
            throw new BusinessRuleException("Customer is not active");

        var existingOrder = await _orderRepo.GetByReferenceAsync(cmd.Reference);
        if (existingOrder != null)
            throw new ConflictException("Order already exists");

        var product = await _productRepo.GetByIdAsync(cmd.ProductId);
        if (product == null)
            throw new NotFoundException("Product not found");

        if (product.Stock < cmd.Quantity)
            throw new BusinessRuleException("Insufficient stock");

        try
        {
            var order = new Order(customer.Id, product.Id, cmd.Quantity);
            await _orderRepo.CreateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish event");
                // Swallow? Rethrow? Who decides?
            }

            return new ApiResponse { OrderId = order.Id };
        }
        catch (DbUpdateException ex)
        {
            throw new ConflictException("Duplicate order", ex);
        }
    }
    catch (NotFoundException ex) { return NotFound(ex.Message); }
    catch (BusinessRuleException ex) { return BadRequest(ex.Message); }
    catch (ConflictException ex) { return Conflict(ex.Message); }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error");
        return InternalServerError();
    }
}`;

    const goodCode = `public Task<AxisResult<CreateOrderResponse>> HandleAsync(CreateOrderCommand cmd)
    => customerFactory.GetByIdAsync(cmd.CustomerId)
        .ThenAsync(customer => orderFactory.CreateAsync(new()
        {
            CustomerId = customer.CustomerId,
            ProductId = cmd.ProductId,
            Quantity = cmd.Quantity
        }))
        .ThenAsync(_ => unitOfWork.SaveChangesAsync())
        .TapAsync(order => logger.LogInformation("Order {OrderId} created", order.OrderId))
        .MapAsync(order => new CreateOrderResponse { OrderId = order.OrderId });`;

    return (
        <Layout>
            <h1 className="text-4xl font-bold mb-8 text-blue-400">Why AxisResult?</h1>

            <div className="prose prose-invert max-w-4xl">
                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-4">The Problem</h2>
                    <p className="text-slate-300 mb-4">
                        This is what "enterprise C#" looks like in most codebases:
                    </p>

                    <CodeBlock code={badCode} />

                    <p className="bg-slate-800 p-4 rounded text-slate-300 mb-6">
                        <strong className="text-yellow-400">40 lines. 5 catch blocks. 3 null checks. 2 nested try/catch.</strong> And the actual business intent is buried under defensive ceremony.
                    </p>

                    <h3 className="text-2xl font-bold text-blue-300 mb-3">The Real Issue</h3>
                    <ul className="text-slate-300 space-y-3 mb-6">
                        <li className="flex gap-3">
                            <span className="text-red-400 font-bold">✗</span>
                            <span>Exceptions are <strong>goto statements in disguise</strong>. They break your call stack.</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-red-400 font-bold">✗</span>
                            <span>They're <strong>invisible in method signatures</strong>, forcing you to guess what might fail.</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-red-400 font-bold">✗</span>
                            <span>They make <strong>every caller responsible</strong> for catching things that aren't exceptional at all.</span>
                        </li>
                        <li className="flex gap-3">
                            <span className="text-red-400 font-bold">✗</span>
                            <span>"Customer not found" <strong>isn't an exception</strong> — it's a perfectly normal outcome.</span>
                        </li>
                    </ul>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-4">The AxisResult Solution</h2>
                    <p className="text-slate-300 mb-4">
                        Now look at the same logic with AxisResult:
                    </p>

                    <CodeBlock code={goodCode} highlight={true} />

                    <p className="bg-blue-900/30 p-4 rounded text-slate-300 mb-6 border border-blue-800">
                        <strong className="text-green-400">5 lines. Zero try/catch. Zero null checks. Zero exceptions.</strong> Every possible failure is encoded in the return type. The pipeline reads like a sentence describing what the operation does.
                    </p>

                    <p className="text-slate-300 text-lg font-semibold text-blue-300">
                        That's Railway-Oriented Programming.
                    </p>
                </section>

                <section className="mb-12">
                    <h2 className="text-3xl font-bold text-blue-300 mb-4">Key Principles</h2>
                    <div className="space-y-4">
                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2">1. Errors Are First-Class Citizens</h4>
                            <p className="text-slate-300">Failures are encoded in the return type, not hidden in exceptions.</p>
                        </div>
                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2">2. Composable Pipelines</h4>
                            <p className="text-slate-300">Chain operations without boilerplate. Each step either succeeds and flows forward, or short-circuits on failure.</p>
                        </div>
                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2">3. Explicit Intent</h4>
                            <p className="text-slate-300">Method signatures show what can fail. No surprises. No hidden side effects.</p>
                        </div>
                        <div className="bg-slate-900 p-4 rounded border border-slate-800">
                            <h4 className="font-bold text-blue-400 mb-2">4. Zero Overhead</h4>
                            <p className="text-slate-300">No external dependencies. No framework weight. Pure, minimal C#.</p>
                        </div>
                    </div>
                </section>
            </div>

            <div className="mt-12 pt-8 border-t border-slate-800">
                <a href="/AxisResult/docs/concepts/" className="text-blue-400 hover:text-blue-300 font-medium text-lg">
                    ← Next: Understand Railway-Oriented Programming
                </a>
            </div>
        </Layout>
    );
};
